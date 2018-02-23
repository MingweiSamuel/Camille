using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MingweiSamuel.Camille.Enums;

namespace MingweiSamuel.Camille.Util
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

        public async Task<T> Get<T>(string methodId, string relativeUrl, Region region,
            KeyValuePair<string, string>[] queryParams, bool nonRateLimited, CancellationToken? token)
        {
            await (token == null ?
                _concurrentRequestSemaphore.WaitAsync() :
                _concurrentRequestSemaphore.WaitAsync(token.Value));
            try
            {
                return await GetRateLimiter(region).Get<T>(methodId, relativeUrl, region, queryParams, nonRateLimited, token);
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
            return _rateLimiters.GetOrAdd(region, r => new RegionalRequester(_config));
        }
    }
}
