using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MingweiSamuel.Camille.Util
{
    public class RateLimit : IRateLimit
    {
        /// <summary>Header specifying which RateLimitType caused a 429.</summary>
        public const string HeaderXRateLimitType = "X-Rate-Limit-Type";
        /// <summary>Header specifying retry after time in seconds after a 429.</summary>
        public const string HeaderRetryAfter = "Retry-After";

        /// <summary>Thread must synchronize on this lock to change bucketsUpdated and buckets.</summary>
        private readonly object _bucketsLock = new object();

        /// <summary>
        /// By default we allow 1 request per second which is actually 1 requests per two seconds due to the
        /// temporal factor of 1. Once a response is successful, we update the buckets to match.
        /// </summary>
        private volatile IReadOnlyList<ITokenBucket> _buckets =
            new List<ITokenBucket> { new CircularBufferTokenBucket(TimeSpan.FromSeconds(1), 1, 1, 0, 1) };

        /// <summary>TickStamp to retry after receiving a 429/Retry-After header.</summary>
        private long _retryAfterTickStamp = 0;

        /// <summary>Type of rate limit, to know what headers to check.</summary>
        private readonly RateLimitType rateLimitType;

        public IReadOnlyList<ITokenBucket> GetBuckets()
        {
            return _buckets;
        }

        public long GetRetryAfterDelay()
        {
            var now = DateTimeOffset.UtcNow.Ticks;
            // Although we check retryAfterTimestamp twice, the timestamp only increases so the
            // retryAfterTimestamp - now value will still be valid.
            return now > _retryAfterTickStamp ? -1 : _retryAfterTickStamp - now;
        }

        public void OnResponse(HttpWebResponse response)
        {
            if (429 == (int) response.StatusCode)
            {
                // Determine if this RateLimit triggered the 429, and set retryAfter accordingly.
                var typeNameHeader = response.GetResponseHeader(HeaderXRateLimitType);
                if (typeNameHeader == null)
                    throw new InvalidOperationException(
                        $"429 response did not include {HeaderXRateLimitType}, indicating a failure of the Riot API edge.");
                if (rateLimitType.TypeName().Equals(typeNameHeader, StringComparison.InvariantCultureIgnoreCase))
                {
                    var retryAfterHeader = response.GetResponseHeader(HeaderRetryAfter);
                    if (retryAfterHeader == null)
                        throw new InvalidOperationException(
                            $"429 response triggered by {rateLimitType.TypeName()} missing {HeaderRetryAfter}" +
                            " header, indicating a failure of the Riot API edge.");
                    // Because the precision of the retryAfter header is only in seconds, we multiply
                    // and add an additional half-second in case of rounding (for example, the API sometimes returns
                    // retry-after 0 seconds).
                    _retryAfterTickStamp = DateTimeOffset.UtcNow.Ticks + TimeSpan.TicksPerSecond * long.Parse(retryAfterHeader)
                        + TimeSpan.TicksPerSecond / 2;
                }
            }

            var limitHeader = response.GetResponseHeader(rateLimitType.LimitHeader());
            var countHeader = response.GetResponseHeader(rateLimitType.CountHeader());
            if (!CheckBucketsRequireUpdating(limitHeader, countHeader))
                return;
            lock(_bucketsLock) {
                if (!CheckBucketsRequireUpdating(limitHeader, countHeader))
                    return;
                try
                {
                    _buckets = GetBucketsFromHeaders(limitHeader, countHeader);
                }
                catch (InvalidOperationException e)
                {
                    throw new RiotResponseException(e, response);
                }
            }
        }

        /// <summary>
        /// Check if the buckets need updating based on a response and the current buckets.
        /// </summary>
        /// <param name="limitHeader"></param>
        /// <param name="countHeader"></param>
        /// <returns>True if needs update, false otherwise.</returns>
        private bool CheckBucketsRequireUpdating(string limitHeader, string countHeader)
        {
            if (limitHeader == null || countHeader == null)
                return false;
            var currentLimit = string.Join(",", _buckets.Select(b => b.ToLimitString()));
            // Needs update if headers do not match.
            return !limitHeader.Equals(currentLimit);
        }

        /// <param name="limitHeader"></param>
        /// <param name="countHeader"></param>
        /// <returns>A new set of buckets based on the provided headers.</returns>
        private IReadOnlyList<ITokenBucket> GetBucketsFromHeaders(String limitHeader, String countHeader)
        {
            // Limits: "20000:10,1200000:600"
            // Counts: "7:10,58:600"
            var limits = limitHeader.Split(',');
            var counts = countHeader.Split(',');
            if (limits.Length != counts.Length)
                throw new InvalidOperationException(
                    $"Header lengths did not match: {limitHeader} and {countHeader}.");
            return limits
                .Zip(counts, (limit, count) =>
                {
                    var limitColon = limit.IndexOf(':');
                    var countColon = count.IndexOf(':');
                    var limitValue = int.Parse(limit.Substring(0, limitColon));
                    var limitSpan = long.Parse(limit.Substring(limitColon + 1));
                    var countValue = int.Parse(count.Substring(0, countColon));
                    var countSpan = long.Parse(count.Substring(countColon + 1));
                    if (limitSpan != countSpan)
                        throw new InvalidOperationException(
                            $"Header timespans did not match: {limitHeader} and {countHeader}.");
                    var bucket = new CircularBufferTokenBucket(TimeSpan.FromSeconds(limitSpan), limitValue, 20, 0.5f, 0.95f);
                    bucket.GetTokens((int) Math.Ceiling(countValue * 1f)); // TODO
                    return bucket;
                })
                .ToList();
        }
    }
}
