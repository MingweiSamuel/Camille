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

        /// <summary>
        /// Certification callback used to certify HTTP requests to the LCU.
        /// </summary>
        public RemoteCertificateValidationCallback CertificateValidationCallback =
            RiotCertificateUtils.CertificateValidationCallback;

        /// <summary>
        /// Rate-limiting token bucket used for requests to the LCU.
        /// </summary>
        public Func<ITokenBucket?> TokenBucketProvider =
            () => new CircularTokenBucket(TimeSpan.FromSeconds(10), 1000, 20, 0.5f, 1.0f);

        /// <summary>
        /// Hostname (IP address or domain, excluding the port) of the LCU.
        /// This should almost certainly be left to the default ("127.0.0.1").
        /// </summary>
        public string Hostname = "127.0.0.1";
    }
}
