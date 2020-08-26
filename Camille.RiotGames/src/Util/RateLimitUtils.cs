using System.Linq;

namespace Camille.RiotGames.Util
{
    public static class RateLimitUtils
    {
        public static long GetOrDelay(params IRateLimit[] rateLimits)
        {
            var retryAfterDelay = rateLimits.Select(r => r.GetRetryAfterDelay()).Max();
            
            // This may not actually be the minimum time needed to delay, if other rateLimits are at their limit
            // due to reasons besides the retry-after, however we ignore that case for simplicity. If this happens,
            // the request will try again and be delayed again.
            if (retryAfterDelay >= 0)
                return retryAfterDelay;

            // Join all buckets into an array and use TemporalBucket.getAllTokensOrDelay().
            return TokenBucketUtils.GetAllTokensOrDelay(rateLimits.SelectMany(r => r.GetBuckets()).ToArray());
        }
    }
}
