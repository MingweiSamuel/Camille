using MingweiSamuel.TokenBucket;
using System;

namespace MingweiSamuel.Camille.Util
{
    public delegate ITokenBucket TokenBucketFactory(TimeSpan timeSpan, int totalLimit, float concurrentInstanceFactor, float overheadFactor);
}
