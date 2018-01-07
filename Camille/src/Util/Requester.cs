using System.Collections.Concurrent;
using System.Threading;
using MingweiSamuel.Camille.Enums;
using MingweiSamuel.Camille.src.Util;

namespace MingweiSamuel.Camille.Util
{
    public class Requester
    {
        /// <summary>Root url for Riot API requests.</summary>
        private const string RiotRootUrl = "%s.api.riotgames.com";
        /// <summary>Lock on number of concurrent requests (global across regions).</summary>
        private readonly SemaphoreSlim _concurrentRequestSemaphore;

        /// <summary>Stores the RateLimiter for each Region.</summary>
        private readonly ConcurrentDictionary<Region, RegionLimiter> _rateLimiters = new ConcurrentDictionary<Region, RegionLimiter>();
    }
}
