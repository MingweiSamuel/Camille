using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using MingweiSamuel.TokenBucket;

namespace Camille.Lcu
{
    public class LcuRequester : IDisposable
    {
        /// <summary>Basic auth username used by the LCU.</summary>
        private const string USERNAME = "riot";

        private readonly LcuConfig _config;
        private readonly ITokenBucket? _tokenBucket;

        private readonly SemaphoreSlim? _concurrentRequestSemaphore;
        private readonly HttpClient _client;

        public LcuRequester(LcuConfig config)
        {
            _config = config;
            _tokenBucket = config.TokenBucketProvider();

            _concurrentRequestSemaphore = _config.MaxConcurrentRequests <= 0
                ? null
                : new SemaphoreSlim(_config.MaxConcurrentRequests);

            var lf = _config.Lockfile ?? Lockfile.Parse(_config.LeagueInstallDir + @"\lockfile");
            _client = new HttpClient(_config.HttpClientHandler);
            _client.BaseAddress = new UriBuilder(lf.Protocol, "127.0.0.1", lf.Port).Uri;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{USERNAME}:{lf.Password}")));
        }

        public async Task<T> SendAsync<T>(HttpRequestMessage request)
        {
            if (null != _concurrentRequestSemaphore)
                await _concurrentRequestSemaphore.WaitAsync();
            try
            {
                if (null != _tokenBucket)
                {
                    long delay;
                    while (0 <= (delay = TokenBucketUtils.GetAllTokensOrDelay(_tokenBucket)))
                        await Task.Delay(TimeSpan.FromTicks(delay));
                }

                var response = await _client.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
#if USE_NEWTONSOFT
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
#endif
#if USE_SYSTEXTJSON
                return System.Text.Json.JsonSerializer.Deserialize<T>(json);
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
        }
    }
}
