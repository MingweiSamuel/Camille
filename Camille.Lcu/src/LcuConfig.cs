using MingweiSamuel.TokenBucket;
using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Camille.Lcu
{
    public class LcuConfig
    {
        /// <summary>Root certificate used by the certificate used by the LCU.</summary>
        public static readonly X509Certificate2 RIOT_CA =
            new X509Certificate2(Convert.FromBase64String(
            "MIIEIDCCAwgCCQDJC+QAdVx4UDANBgkqhkiG9w0BAQUFADCB0TELMAkGA1UEBhMC" +
            "VVMxEzARBgNVBAgTCkNhbGlmb3JuaWExFTATBgNVBAcTDFNhbnRhIE1vbmljYTET" +
            "MBEGA1UEChMKUmlvdCBHYW1lczEdMBsGA1UECxMUTG9MIEdhbWUgRW5naW5lZXJp" +
            "bmcxMzAxBgNVBAMTKkxvTCBHYW1lIEVuZ2luZWVyaW5nIENlcnRpZmljYXRlIEF1" +
            "dGhvcml0eTEtMCsGCSqGSIb3DQEJARYeZ2FtZXRlY2hub2xvZ2llc0ByaW90Z2Ft" +
            "ZXMuY29tMB4XDTEzMTIwNDAwNDgzOVoXDTQzMTEyNzAwNDgzOVowgdExCzAJBgNV" +
            "BAYTAlVTMRMwEQYDVQQIEwpDYWxpZm9ybmlhMRUwEwYDVQQHEwxTYW50YSBNb25p" +
            "Y2ExEzARBgNVBAoTClJpb3QgR2FtZXMxHTAbBgNVBAsTFExvTCBHYW1lIEVuZ2lu" +
            "ZWVyaW5nMTMwMQYDVQQDEypMb0wgR2FtZSBFbmdpbmVlcmluZyBDZXJ0aWZpY2F0" +
            "ZSBBdXRob3JpdHkxLTArBgkqhkiG9w0BCQEWHmdhbWV0ZWNobm9sb2dpZXNAcmlv" +
            "dGdhbWVzLmNvbTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKoJemF/" +
            "6PNG3GRJGbjzImTdOo1OJRDI7noRwJgDqkaJFkwv0X8aPUGbZSUzUO23cQcCgpYj" +
            "21ygzKu5dtCN2EcQVVpNtyPuM2V4eEGr1woodzALtufL3Nlyh6g5jKKuDIfeUBHv" +
            "JNyQf2h3Uha16lnrXmz9o9wsX/jf+jUAljBJqsMeACOpXfuZy+YKUCxSPOZaYTLC" +
            "y+0GQfiT431pJHBQlrXAUwzOmaJPQ7M6mLfsnpHibSkxUfMfHROaYCZ/sbWKl3lr" +
            "ZA9DbwaKKfS1Iw0ucAeDudyuqb4JntGU/W0aboKA0c3YB02mxAM4oDnqseuKV/CX" +
            "8SQAiaXnYotuNXMCAwEAATANBgkqhkiG9w0BAQUFAAOCAQEAf3KPmddqEqqC8iLs" +
            "lcd0euC4F5+USp9YsrZ3WuOzHqVxTtX3hR1scdlDXNvrsebQZUqwGdZGMS16ln3k" +
            "WObw7BbhU89tDNCN7Lt/IjT4MGRYRE+TmRc5EeIXxHkQ78bQqbmAI3GsW+7kJsoO" +
            "q3DdeE+M+BUJrhWorsAQCgUyZO166SAtKXKLIcxa+ddC49NvMQPJyzm3V+2b1roP" +
            "SvD2WV8gRYUnGmy/N0+u6ANq5EsbhZ548zZc+BI4upsWChTLyxt2RxR7+uGlS1+5" +
            "EcGfKZ+g024k/J32XP4hdho7WYAS2xMiV83CfLR/MNi8oSMaVQTdKD8cpgiWJk3L" +
            "XWehWA=="));

        /// <summary>
        /// League install dir. Will be ignored if lockfile is set.
        /// </summary>
        public string LeagueInstallDir = @"C:\Riot Games\League of Legends";

        /// <summary>
        /// Lockfile, optional. If not set, will try to read existing lockfile in LeagueInstallDir.
        /// </summary>
        public Lockfile? Lockfile = null;

        /// <summary>
        /// Max concurrent requests (per LCU).
        /// </summary>
        public int MaxConcurrentRequests = 100;

        /// <summary>
        /// Handler used for SSL.
        /// </summary>
        public HttpClientHandler HttpClientHandler = new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (req, cert, chain, polErrs) => {
                // Normal verification.
                if (SslPolicyErrors.None == polErrs)
                    return true;

                // Private chain for our root cert.
                using X509Chain privateChain = new X509Chain();
                // Do not use `AllowUnknownCertificateAuthority` (ignores `ExtraStore`).
                // https://stackoverflow.com/questions/6097671/how-to-verify-x509-cert-without-importing-root-cert
                privateChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                privateChain.ChainPolicy.ExtraStore.Add(RIOT_CA); // Add root certificate.
                privateChain.Build(cert);

                // Only error should be untrusted root certificate.
                // (Chain error if root isn't our root).
                return 1 == privateChain.ChainStatus.Length &&
                    privateChain.ChainStatus[0].Status == X509ChainStatusFlags.UntrustedRoot;
            }
        };

        public Func<ITokenBucket?> TokenBucketProvider = () => new CircularTokenBucket(TimeSpan.FromSeconds(10), 1000, 20, 0.5f, 1.0f);
    }
}
