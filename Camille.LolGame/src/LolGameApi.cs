using Camille.Core;
using MingweiSamuel.TokenBucket;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Camille.LolGame
{
    public class LolGameApi : ILolGameApi
    {
        private readonly LolGameConfig _config;

        private readonly ITokenBucket? _tokenBucket;

        private readonly SemaphoreSlim? _concurrentRequestSemaphore;
        private readonly HttpClientHandler _clientHandler;
        /// <summary>
        /// HttpClient for sending requests. Better than using WebClient in this case.
        /// https://stackoverflow.com/a/27737601/2398020
        /// </summary>
        private readonly HttpClient _client = new HttpClient();

        public LolGameApi() : this(new LolGameConfig()) {}

        public LolGameApi(LolGameConfig config)
        {
            _config = config;

            _tokenBucket = config.TokenBucketProvider();

            _concurrentRequestSemaphore = config.MaxConcurrentRequests <= 0
                ? null
                : new SemaphoreSlim(config.MaxConcurrentRequests);

            _clientHandler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (req, cert, chain, polErrs) =>
                    RiotCertificateUtils.CertificateValidationCallback(req, cert, chain, polErrs)
            };

            _client = new HttpClient(_clientHandler)
            {
                BaseAddress = _config.BaseAddress
            };
        }

        public async Task<T> Send<T>(HttpRequestMessage request, CancellationToken? token = null)
        {
            await new SynchronizationContextRemover();
            var content = await SendInternal(request, token.GetValueOrDefault());
            if (null == content) return default!; // TODO: throw exception on unexpected null.
            return JsonHandler.Deserialize<T>(content);
        }

        public async Task Send(HttpRequestMessage request, CancellationToken? token = null)
        {
            await new SynchronizationContextRemover();
            await SendInternal(request, token.GetValueOrDefault());
        }

        private async Task<string?> SendInternal(HttpRequestMessage request, CancellationToken token)
        {
            if (null != _concurrentRequestSemaphore)
                await _concurrentRequestSemaphore.WaitAsync(token);

            HttpResponseMessage? response = null;
            string? responseContent = null;
            LolGameErrorMessage? errorMessage = null;
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

                using (request)
                    response = await _client.SendAsync(request, token);

                if (HttpStatusCode.NotFound == response.StatusCode || HttpStatusCode.NoContent == response.StatusCode)
                    return null;
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();

                responseContent = await response.Content.ReadAsStringAsync();
                errorMessage = JsonHandler.Deserialize<LolGameErrorMessage>(responseContent);
                if (_config.IsNullResponse(errorMessage))
                    return null;
            }
            catch (Exception e)
            {
                throw new LolGameException(e, response, errorMessage, responseContent);
            }
            finally
            {
                _concurrentRequestSemaphore?.Release();
            }
            throw new LolGameException(response, errorMessage, responseContent);
        }
    }
}
