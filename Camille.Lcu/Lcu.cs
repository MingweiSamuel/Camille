using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Reflection;
using System.Net.Security;

namespace Camille.Lcu
{
    public class Lcu : IDisposable
    {
        /// <summary>Basic auth username used by the LCU.</summary>
        private const string USERNAME = "riot";
        /// <summary>Root certificate used by the certificate used by the LCU.</summary>
        private static readonly X509Certificate2 RIOT_CERT =
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

        private readonly Lockfile _lockfile;

        private readonly HttpClientHandler _clientHandler;
        private readonly HttpClient _client;

        public Lcu(Lockfile lockfile)
        {
            _lockfile = lockfile;

            _clientHandler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (req, cert, chain, polErrs) => {
                    // Normal verification.
                    if (SslPolicyErrors.None == polErrs)
                        return true;

                    using X509Chain privateChain = new X509Chain();
                    // Do not use `AllowUnknownCertificateAuthority` (ignores `ExtraStore`).
                    privateChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                    privateChain.ChainPolicy.ExtraStore.Add(RIOT_CERT); // Add root certificate.
                    privateChain.Build(cert);

                    // Only error should be that the root certificate is untrusted.
                    return 1 == privateChain.ChainStatus.Length &&
                        privateChain.ChainStatus[0].Status == X509ChainStatusFlags.UntrustedRoot;
                }
            };

            _client = new HttpClient(_clientHandler);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{USERNAME}:{lockfile.Password}")));
            Console.WriteLine(Convert.ToBase64String(Encoding.ASCII.GetBytes($"{USERNAME}:{lockfile.Password}")));
            _client.BaseAddress = new UriBuilder(lockfile.Protocol, "127.0.0.1", lockfile.Port).Uri;
        }

        public async Task<string> SendRequestTest()
        {
            var response = await _client.GetAsync("/riotclient/region-locale");
            Console.WriteLine(response.Headers);
            Console.WriteLine(response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        public void Dispose()
        {
            _client.Dispose();
            _clientHandler.Dispose();
        }
    }
}
