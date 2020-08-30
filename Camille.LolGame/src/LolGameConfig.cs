using Camille.Core;
using MingweiSamuel.TokenBucket;
using System;
using System.Net.Security;

namespace Camille.LolGame
{
    public class LolGameConfig
    {
        /// <summary>
        /// Max concurrent requests.
        /// </summary>
        public int MaxConcurrentRequests = 100;

        public RemoteCertificateValidationCallback CertificateValidationCallback =
            RiotCertificateUtils.CertificateValidationCallback;

        public Func<ITokenBucket?> TokenBucketProvider =
            () => new CircularTokenBucket(TimeSpan.FromSeconds(10), 1000, 20, 0.5f, 1.0f);

        public Func<LolGameErrorMessage, bool> IsNullResponse =
            errorMessage => "Spectator mode doesn't currently support this feature".Equals(errorMessage.Message);
    }
}
