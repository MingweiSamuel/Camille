using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Camille.Core;
using Camille.RiotGames.Util;

namespace Camille.RiotGames
{
    public class RiotGamesApi : IRiotGamesApi
    {
        /// <summary>RequestManager for sending web requests and managing rate limits.</summary>
        private readonly RequestManager _requestManager;

        private RiotGamesApi(IRiotGamesApiConfig config)
        {
            if (null == config) throw new ArgumentException($"{nameof(config)} cannot be null.");
            _requestManager = new RequestManager(config);
        }

        /// <summary>
        /// Creates a new RiotApi instance with default configuration.
        /// </summary>
        /// <param name="apiKey">Riot API key.</param>
        /// <returns>RiotApi instance using supllied apiKey.</returns>
        public static RiotGamesApi NewInstance(string apiKey)
        {
            return NewInstance(new RiotGamesApiConfig.Builder(apiKey).Build());
        }

        /// <summary>
        /// Creates a new RiotApi instance with the provided configuration.
        /// </summary>
        /// <param name="config">RiotApiConfig to use.</param>
        /// <returns>RiotApi instance using supplied configuration.</returns>
        public static RiotGamesApi NewInstance(IRiotGamesApiConfig config)
        {
            return new RiotGamesApi(config);
        }

        public async Task<T> Send<T>(string route, string methodId, HttpRequestMessage request,
            CancellationToken? token = null, bool ignoreAppRateLimits = false)
        {
            // Camille's code is context-free.
            // This slightly improves performance and helps prevent GUI thread deadlocks.
            // https://blogs.msdn.microsoft.com/benwilli/2017/02/09/an-alternative-to-configureawaitfalse-everywhere/
            await new SynchronizationContextRemover();
            var content = await _requestManager.Send(route, methodId, request, token.GetValueOrDefault(), ignoreAppRateLimits);
            if (null == content) return default!; // TODO: throw exception on unexpected null.
            return JsonHandler.Deserialize<T>(content);
        }

        public async Task Send(string route, string methodId, HttpRequestMessage request,
            CancellationToken? token = null, bool ignoreAppRateLimits = false)
        {
            await new SynchronizationContextRemover();
            await _requestManager.Send(route, methodId, request, token.GetValueOrDefault(), ignoreAppRateLimits);
        }
    }
}
