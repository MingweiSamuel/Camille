using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Camille.Enums;

namespace Camille.RiotApi.Util
{
    /// <summary>
    /// For sending rate-limited requests to the Riot API.
    /// 
    /// Manages splitting up regions and limiting concurrent connections.
    /// </summary>
    public class RequestManager
    {
        /// <summary>Configuration information.</summary>
        private readonly IRiotApiConfig _config;

        /// <summary>Lock on number of concurrent requests (global across regions).</summary>
        private readonly SemaphoreSlim _concurrentRequestSemaphore;

        /// <summary>Stores the RateLimiter for each Region.</summary>
        private readonly ConcurrentDictionary<string, RegionalRequester> _rateLimiters = new ConcurrentDictionary<string, RegionalRequester>();

        public RequestManager(IRiotApiConfig config)
        {
            _config = config;
            _concurrentRequestSemaphore = new SemaphoreSlim(_config.MaxConcurrentRequests);
        }

        public async Task<string?> Send(string route, string methodId, HttpRequestMessage request,
            CancellationToken token, bool ignoreAppRateLimits)
        {
            await _concurrentRequestSemaphore.WaitAsync(token);
            try
            {
                return await GetRateLimiter(route).Send(methodId, request, token, ignoreAppRateLimits);
            }
            finally
            {
                _concurrentRequestSemaphore.Release();
            }
        }

        /// <summary>
        /// Gets a rate limiter from a region, creating it if needed.
        /// </summary>
        /// <param name="route">Route subdomain corresponding to a region or platform.</param>
        /// <returns>The rate limiter.</returns>
        private RegionalRequester GetRateLimiter(string route)
        {
            return _rateLimiters.GetOrAdd(route, r => new RegionalRequester(_config, route));
        }
    }
}
