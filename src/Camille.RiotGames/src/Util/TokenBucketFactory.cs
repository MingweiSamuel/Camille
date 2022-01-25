using MingweiSamuel.TokenBucket;
using System;

namespace Camille.RiotGames.Util
{
    public delegate ITokenBucket TokenBucketFactory(TimeSpan timeSpan, int totalLimit, float concurrentInstanceFactor, float overheadFactor);
}
