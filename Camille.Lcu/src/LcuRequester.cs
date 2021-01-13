using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MingweiSamuel.TokenBucket;
using Camille.Core;

namespace Camille.Lcu
{
    public class LcuRequester : IDisposable
    {
        /// <summary>Basic auth username used by the LCU.</summary>
        private const string USERNAME = "riot";

        private readonly ITokenBucket? _tokenBucket;

        private readonly SemaphoreSlim? _concurrentRequestSemaphore;
        private readonly HttpClientHandler _clientHandler;
        private readonly HttpClient _client;

        public LcuRequester(Lockfile lockfile, LcuConfig config)
        {
            _tokenBucket = config.TokenBucketProvider();

            _concurrentRequestSemaphore = config.MaxConcurrentRequests <= 0
                ? null
                : new SemaphoreSlim(config.MaxConcurrentRequests);

            _clientHandler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (req, cert, chain, polErrs) =>
                    config.CertificateValidationCallback(req, cert, chain, polErrs)
            };

            _client = new HttpClient(_clientHandler);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{USERNAME}:{lockfile.Password}")));
            _client.BaseAddress = new UriBuilder(lockfile.Protocol, config.Hostname, lockfile.Port).Uri;
        }

        public async Task<string> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            if (null != _concurrentRequestSemaphore)
                await _concurrentRequestSemaphore.WaitAsync(token);
            try
            {
                if (null != _tokenBucket)
                {
                    long delay;
                    while (0 <= (delay = TokenBucketUtils.GetAllTokensOrDelay(_tokenBucket)))
                    {
                        await Task.Delay(TimeSpan.FromTicks(delay), token);
                        token.ThrowIfCancellationRequested();
                    }
                }

                HttpResponseMessage response;
                using (request)
                    response = await _client.SendAsync(request, token);

#if USE_HTTPCONTENT_READASSTRINGASYNC_CANCELLATIONTOKEN
                return await response.Content.ReadAsStringAsync(token);
#else
                return await response.Content.ReadAsStringAsync();
#endif
            }
            finally
            {
                _concurrentRequestSemaphore?.Release();
            }
        }

        public void Dispose()
        {
            _client.Dispose();
            _clientHandler.Dispose();
        }
    }
}
