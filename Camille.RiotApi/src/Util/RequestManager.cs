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
        private readonly ConcurrentDictionary<Region, RegionalRequester> _rateLimiters = new ConcurrentDictionary<Region, RegionalRequester>();

        public RequestManager(IRiotApiConfig config)
        {
            _config = config;
            _concurrentRequestSemaphore = new SemaphoreSlim(_config.MaxConcurrentRequests);
        }

        public async Task<T> Send<T>(Region region, string methodId, bool nonRateLimited, 
            HttpRequestMessage request, CancellationToken? token)
        {
            await _concurrentRequestSemaphore.WaitAsync(token.GetValueOrDefault());
            try
            {
                return await GetRateLimiter(region).Send<T>(methodId, nonRateLimited, request, token);
            }
            finally
            {
                _concurrentRequestSemaphore.Release();
            }
        }

        /// <summary>
        /// Gets a rate limiter from a region, creating it if needed.
        /// </summary>
        /// <param name="region">Region of rate limiter to get.</param>
        /// <returns>The rate limiter.</returns>
        private RegionalRequester GetRateLimiter(Region region)
        {
            return _rateLimiters.GetOrAdd(region, r => new RegionalRequester(_config, region));
        }
    }
}
