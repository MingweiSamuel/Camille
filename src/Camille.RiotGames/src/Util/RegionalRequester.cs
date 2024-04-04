using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Camille.Core;
using Camille.RiotGames.Enums;

namespace Camille.RiotGames.Util
{
    /// <summary>
    /// Manages rate limits for a particular region and sends requests.
    /// Retries retryable responses (429, 5xx).
    /// Processes non-retryable responses (200, 404, 4xx).
    /// </summary>
    public class RegionalRequester
    {
        /// <summary>Request header name for the Riot API key.</summary>
        private const string RiotKeyHeader = "X-Riot-Token";

        /// <summary>Configuration information.</summary>
        private readonly IRiotGamesApiConfig _config;

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

        /// <summary>
        /// The region/route for this requester.
        /// </summary>
        private string _route;

        public RegionalRequester(IRiotGamesApiConfig config, string route)
        {
            _config = config;
            _appRateLimit = new RateLimit(RateLimitType.Application, config);
            _route = route;

            // If the region is to be in the url.
            if (_config.ApiRouteConfig == RouteConfig.InUrl)
            {
                _client.BaseAddress = new Uri(string.Format(config.ApiUrl, route));
            }
            // If the region is to be in the url query parameters or headers.
            else
            {
                _client.BaseAddress = new Uri(config.ApiUrl);
            }

            // If the region is to be in the header.
            if (_config.ApiRouteConfig == RouteConfig.InHeader)
            {
                _client.DefaultRequestHeaders.Add(config.RouteKey, route);
            }

            // Include the API key (can set config.ApiKey to "" for use with proxies).
            _client.DefaultRequestHeaders.Add(RiotKeyHeader, config.ApiKey);
        }

        /// <summary>
        /// HttpStatus codes that are considered a success, but will return null (or default(T)).
        /// Listed from most common to least common.
        /// </summary>
        private static readonly int[] NullSuccessStatusCodes = { 404, 204, 422 };

        /// <summary>
        /// Sends a GET request, obeying rate limits and retry afters.
        /// </summary>
        /// <param name="methodId"></param>
        /// <param name="request">Request to send (use relative url).</param>
        /// <param name="token">CancellationToken to cancel this task.</param>
        /// <param name="ignoreAppRateLimits">If set to true, the request will not count against the application rate limit.</param>
        /// <returns>Response body (or null if no body).</returns>
        public async Task<string?> Send(string methodId, HttpRequestMessage request,
            CancellationToken token, bool ignoreAppRateLimits)
        {
            // If the route is in the query param we need to add it to each request.
            if (_config.ApiRouteConfig == RouteConfig.InQueryParam)
            {
                // Append the route as a query parameter.
                var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
                query.Add(_config.RouteKey, _route);
                // Use path-only URL, scheme/host is in _client.BaseAddress.
                request.RequestUri = new Uri($"{request.RequestUri.AbsolutePath}?{query}", UriKind.RelativeOrAbsolute);
            }

            HttpResponseMessage? response = null;
            var retries = 0;
            var num429s = 0;
            var num5xxs = 0;

            for (; retries <= _config.Retries; retries++)
            {
                // Get token.
                var methodRateLimit = GetMethodRateLimit(methodId);
                long delay;
                var rateLimits = ignoreAppRateLimits ? new[] { methodRateLimit } : new[] { _appRateLimit, methodRateLimit };
                while (0 <= (delay = RateLimitUtils.GetOrDelay(rateLimits)))
                {
                    await Task.Delay(TimeSpan.FromTicks(delay), token);
                    token.ThrowIfCancellationRequested();
                }

                // Send request, receive response.
                // Ensure request is disposed for good measure.
                using var sentRequest = request;
                response = await _client.SendAsync(sentRequest, token);
                var backoffSeconds = _config.BackoffStrategy(retries, num429s, num5xxs);
                foreach (var rateLimit in rateLimits)
                    rateLimit.OnResponse(response, backoffSeconds);

                // Success.
                if (HttpStatusCode.OK == response.StatusCode)
                {
#if USE_HTTPCONTENT_READASSTRINGASYNC_CANCELLATIONTOKEN
                    return await response.Content.ReadAsStringAsync(token);
#else
                    return await response.Content.ReadAsStringAsync();
#endif
                }
                // Null success (no body).
                if (0 <= Array.IndexOf(NullSuccessStatusCodes, (int) response.StatusCode))
                    return default;
                // Failure. 429 and 5xx are retryable. All else exit (break loop).
                var is429 = 429 == (int) response.StatusCode;
                if (is429)
                    num429s++;
                if (is429 || HttpStatusCode.InternalServerError <= response.StatusCode)
                {
                    request = HttpRequestMessageUtils.Copy(sentRequest);
                    await Task.Delay(TimeSpan.FromSeconds(backoffSeconds), token);
                    continue;
                }
                break;
            }
            throw new RiotResponseException(
                $"Request to {methodId} failed after {retries} retries. " +
                $"(status: {(int) (response?.StatusCode ?? 0)}).", response);
        }

        private IRateLimit GetMethodRateLimit(string methodId)
        {
            return _methodRateLimits.GetOrAdd(methodId, (m, c) => new RateLimit(RateLimitType.Method, c), _config);
        }
    }
}
