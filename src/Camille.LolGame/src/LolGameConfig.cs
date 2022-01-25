using Camille.Core;
using MingweiSamuel.TokenBucket;
using System;
using System.Net.Security;

namespace Camille.LolGame
{
    public class LolGameConfig
    {
        /// <summary>Max concurrent requests.</summary>
        public int MaxConcurrentRequests = 100;

        /// <summary>Base address of game client API.</summary>
        public Uri BaseAddress = new Uri("https://127.0.0.1:2999");

        /// <summary>Certificate validation callback to handle Riot-signed cert.</summary>
        public RemoteCertificateValidationCallback CertificateValidationCallback =
            RiotCertificateUtils.CertificateValidationCallback;

        // TODO: only need a single token bucket?
        /// <summary>Function to provide a token bucket</summary>
        public Func<ITokenBucket?> TokenBucketProvider =
            () => new CircularTokenBucket(TimeSpan.FromSeconds(10), 1000, 20, 0.5f, 1.0f);

        /// <summary>Handler to determine if an error should be converted to null.</summary>
        public Func<LolGameErrorMessage, bool> IsNullResponse =
            errorMessage => "Spectator mode doesn't currently support this feature".Equals(errorMessage.Message);
    }
}
