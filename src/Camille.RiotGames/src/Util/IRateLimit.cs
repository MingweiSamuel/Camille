using MingweiSamuel.TokenBucket;
using System.Collections.Generic;
using System.Net.Http;

namespace Camille.RiotGames.Util
{
    public interface IRateLimit
    {
        /// <summary>
        ///  Get delay needed to respect retry-after headers.
        /// </summary>
        /// <returns>Ticks to delay or -1 if none needed. Zero may be returned.</returns>
        long GetRetryAfterDelay();

        /// <summary>
        /// Get the rate limit's buckets.
        /// </summary>
        /// <returns>Current buckets.</returns>
        IReadOnlyList<ITokenBucket> GetBuckets();

        /// <summary>
        /// Callback for when a response returns. Used to update RetryAfter delay and rate limit buckets.
        /// </summary>
        /// <param name="response">Response that applies to this RateLimit.</param>
        /// <param name="defaultRetrySeconds">Seconds to delay if response is 429 and has missing/invalid Retry-After header.</param>
        void OnResponse(HttpResponseMessage response, double defaultRetrySeconds);
    }
}
