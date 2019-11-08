using MingweiSamuel.TokenBucket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MingweiSamuel.Camille.Util
{
    public static class TokenBucketUtils
    {
        public static string ToLimitString(this ITokenBucket b)
        {
            return b.GetTotalLimit() + ":" + (b.GetTickSpan() / TimeSpan.TicksPerSecond);
        }

        public static bool AreEquivalent(ITokenBucket a, ITokenBucket b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            return a.GetTickSpan() == b.GetTickSpan() && a.GetTotalLimit() == b.GetTotalLimit();
        }

        /// <summary>
        /// Attempts to get a token from every bucket, or no tokens at all. Will synchronize on each instance
        /// recursively.
        /// </summary>
        /// <param name="buckets">Buckets to get tokens from.</param>
        /// <returns>-1 if tokens were obtained, otherwise the approximate delay until tokens will be available.</returns>
        public static long GetAllTokensOrDelay(params ITokenBucket[] buckets)
        {
            // Always obtain locks in well-defined order to prevent deadlock. Sort by hash code.
            Array.Sort(buckets, (x, y) => x.GetHashCode() - y.GetHashCode());
            var i = GetAllInternal(buckets, 0);
            if (i < 0) // Success
                return -1;
            // If there was delay, find the maximum or zero. This may be inaccurate due to buckets changing state
            // but that is inevitable unless we block the locks, but its better to let other threads through.
            // Skip i because we know from GetAllInternal that the ith bucket was the first with delay.
            return buckets.Skip(i).Max(b => b.GetDelay());
        }

        /// <summary>
        /// </summary>
        /// <param name="buckets">Buckets to check.</param>
        /// <param name="i">Index of current bucket(for recursion).</param>
        /// <returns>-1 if all tokens were obtained or the index of the first limiting bucket.</returns>
        private static int GetAllInternal(IReadOnlyList<ITokenBucket> buckets, int i)
        {
            // Base case: No more buckets.
            if (i >= buckets.Count)
                return -1;
            // Synchronize on the current bucket.
            lock(buckets[i]) {
                var delay = buckets[i].GetDelay();
                if (delay >= 0) // This bucket has delay, exit synchronization immediately.
                    return i;
                // A token is available for the current bucket.
                var index = GetAllInternal(buckets, i + 1);
                if (index >= 0) // A later bucket has delay, exit synchronization immediately.
                    return index;
                // Success.
                buckets[i].GetTokens(1);
                return -1;
            }
        }
    }
}
