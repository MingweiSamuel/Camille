using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Camille.Enums;

namespace Camille.RiotApi.Util
{
    /// <summary>
    /// Manages rate limits for a particular region and sends requests.
    /// Retries retryable responses (429, 5xx).
    /// Processes non-retryable responses (200, 404, 4xx).
    /// </summary>
    public class RegionalRequester
    {
        /// <summary>Root url for Riot API requests.</summary>
        private const string RiotRootUrl = ".api.riotgames.com";
        /// <summary>Request header name for the Riot API key.</summary>
        private const string RiotKeyHeader = "X-Riot-Token";

        /// <summary>Configuration information.</summary>
        private readonly IRiotApiConfig _config;

        /// <summary>Represents the app rate limit.</summary>
        private readonly IRateLimit _appRateLimit;

        /// <summary>Represents method rate limits.</summary>
        private readonly ConcurrentDictionary<string, IRateLimit> _methodRateLimits =
            new ConcurrentDictionary<string, IRateLimit>();

        /// <summary>
        /// HttpClient for sending requests. Better than using WebClient in this case.
        /// https://stackoverflow.com/a/27737601/2398020
        /// </summary>
        private readonly HttpClient _client = new HttpClient();

        public RegionalRequester(IRiotApiConfig config, Region region)
        {
            _config = config;
            _appRateLimit = new RateLimit(RateLimitType.Application, config);

            _client.BaseAddress = new Uri($"https://{region.ToString()}{RiotRootUrl}");
            _client.DefaultRequestHeaders.Add(RiotKeyHeader, config.ApiKey);
        }

        /// <summary>HttpStatus codes that are considered a success, but will return null (or default(T)).</summary>
        private static readonly int[] NullSuccessStatusCodes = { 204, 404, 422 };

        /// <summary>
        /// Sends a GET request, obeying rate limits and retry afters.
        /// </summary>
        /// <param name="methodId"></param>
        /// <param name="nonRateLimited">If set to true, the request will not count against the application rate limit.</param>
        /// <param name="request">Request to send (use relative url).</param>
        /// <param name="token">CancellationToken to cancel this task.</param>
        /// <returns>Response body (or null if no body).</returns>
        public async Task<string?> Send(string methodId, bool nonRateLimited,
            HttpRequestMessage request, CancellationToken token)
        {
            HttpResponseMessage? response = null;
            var retries = 0;
            for (; retries <= _config.Retries; retries++)
            {
                // Get token.
                var methodRateLimit = GetMethodRateLimit(methodId);
                long delay;
                var rateLimits = nonRateLimited ? new[] { methodRateLimit } : new[] { _appRateLimit, methodRateLimit };
                while (0 <= (delay = RateLimitUtils.GetOrDelay(rateLimits)))
                {
                    await Task.Delay(TimeSpan.FromTicks(delay), token);
                    token.ThrowIfCancellationRequested();
                }

                // Send request, receive response.
                response = await _client.SendAsync(request, token);
                foreach (var rateLimit in rateLimits)
                    rateLimit.OnResponse(response);
                // Success.
                if (HttpStatusCode.OK == response.StatusCode)
                    return await response.Content.ReadAsStringAsync();
                // Null success (no body).
                if (0 <= Array.BinarySearch(NullSuccessStatusCodes, (int) response.StatusCode))
                    return default;
                // Failure. 429 and 5xx are retryable. All else exit.
                if (429 == (int)response.StatusCode || response.StatusCode >= HttpStatusCode.InternalServerError)
                    continue;
                break;
            }
            throw new RiotResponseException(
                $"Request to {methodId} failed after {retries} retries. " +
                $"(status: {(int) (response?.StatusCode ?? 0)}).", response);
        }

        private IRateLimit GetMethodRateLimit(string methodId)
        {
            return _methodRateLimits.GetOrAdd(methodId, m => new RateLimit(RateLimitType.Method, _config));
        }
    }
}
