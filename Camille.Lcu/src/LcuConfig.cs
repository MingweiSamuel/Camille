using Camille.Core;
using MingweiSamuel.TokenBucket;
using System;
using System.Net.Security;

namespace Camille.Lcu
{
    public class LcuConfig
    {
        /// <summary>
        /// Max concurrent requests (per LCU).
        /// </summary>
        public int MaxConcurrentRequests = 100;

        public RemoteCertificateValidationCallback CertificateValidationCallback =
            RiotCertificateUtils.CertificateValidationCallback;

        public Func<ITokenBucket?> TokenBucketProvider =
            () => new CircularTokenBucket(TimeSpan.FromSeconds(10), 1000, 20, 0.5f, 1.0f);
    }
}
