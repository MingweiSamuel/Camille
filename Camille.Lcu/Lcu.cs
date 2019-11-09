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
        /// <summary>Certificate used by the LCU.</summary>
        private static readonly X509Certificate2 RIOT_CERT =
            new X509Certificate2(Convert.FromBase64String(
            "MIIDkjCCAnqgAwIBAgIBYTANBgkqhkiG9w0BAQsFADCB0TELMAkGA1UEBhMCVVMx" +
            "EzARBgNVBAgTCkNhbGlmb3JuaWExFTATBgNVBAcTDFNhbnRhIE1vbmljYTETMBEG" +
            "A1UEChMKUmlvdCBHYW1lczEdMBsGA1UECxMUTG9MIEdhbWUgRW5naW5lZXJpbmcx" +
            "MzAxBgNVBAMTKkxvTCBHYW1lIEVuZ2luZWVyaW5nIENlcnRpZmljYXRlIEF1dGhv" +
            "cml0eTEtMCsGCSqGSIb3DQEJARYeZ2FtZXRlY2hub2xvZ2llc0ByaW90Z2FtZXMu" +
            "Y29tMB4XDTE2MDEwNzIwMDMzM1oXDTI2MDEwNDIwMDMzM1owEjEQMA4GA1UEAwwH" +
            "cmNsaWVudDCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKAEY5b/ofR3" +
            "Hkw3ga5epY6r/XHXtxtnC1aDpFdvaAK4w3XW6wSqkt9p71dA6LJ+IHdA9BR8KXvj" +
            "eOay//SGhSm7Ac7ceGH5s7CK8nt8py6orM6nKTKrt/Ujt2W/px+UGYrQdmQ1v+0s" +
            "/JTqNjXeZV04Y/YtWGvoOhFo4tTYCySbfkWmnz+h9VrM7S6dqZi7ulM9PkLr77oE" +
            "LPhjLJssXZU1RPkv4OAhdXGJhedKI1Gjl1OtRLoNgKJ5Z0ADpJPbq8/bPsHoOeZ8" +
            "bNCWIRTdQQWWBoAdKJ0tMcEipGuaMDKSm62QYnut2xdBUU0QQth8pPVh0T5U0kHk" +
            "OsIs+f8yhYECAwEAAaMzMDEwEwYDVR0lBAwwCgYIKwYBBQUHAwEwGgYDVR0RBBMw" +
            "EYcEfwAAAYIJbG9jYWxob3N0MA0GCSqGSIb3DQEBCwUAA4IBAQB844UGlHduV1uZ" +
            "1A0J9nnotBvqRR1F74Ni/BWMH00L5N+klW2qpVft6+K9mqFgY+A9ym/jHf7w3wlu" +
            "8S7Bdwh/AEm7U21UmxkeViZ1zwfj+iujsjAnLGCZsNoP+Eew2VAhegNVO0cVQ/Tu" +
            "IHFowIF1gmPXkBSi4n0veEhBmsp9k70wCmHSaDJ2nLftWBhC4OMhMUO0HxAO2v5Y" +
            "b1fQPJv5goyhDa5Wnzl4LZimoVcBMX01ll1EolcwZVKIV+SyBexzG5bjGlT6clNG" +
            "V2RHs+ciYoXWtVjzmZ3DqNTvIjnqhcXQQ7n1yXk3JJF1WqkbRUCCOWXxUpuQy8Lp" +
            "6Vy9F980"));

        private readonly Lockfile _lockfile;

        private readonly HttpClientHandler _clientHandler;
        private readonly HttpClient _client;

        public Lcu(Lockfile lockfile)
        {
            _lockfile = lockfile;

            _clientHandler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (req, cert, chain, polErr) =>
                    RIOT_CERT.Equals(cert) && 0 == (polErr & ~SslPolicyErrors.RemoteCertificateChainErrors)
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
