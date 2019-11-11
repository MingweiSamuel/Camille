using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Camille.Enums;
using Camille.RiotApi.Util;

namespace Camille.RiotApi
{
    public partial class RiotApi
    {
        /// <summary>RequestManager for sending web requests and managing rate limits.</summary>
        private readonly RequestManager _requestManager;

        private RiotApi(IRiotApiConfig config) : this()
        {
            if (null == config) throw new ArgumentException($"{nameof(config)} cannot be null.");
            _requestManager = new RequestManager(config);
        }

        /// <summary>
        /// Creates a new RiotApi instance with default configuration.
        /// </summary>
        /// <param name="apiKey">Riot API key.</param>
        /// <returns>RiotApi instance using supllied apiKey.</returns>
        public static RiotApi NewInstance(string apiKey)
        {
            return NewInstance(new RiotApiConfig.Builder(apiKey).Build());
        }

        /// <summary>
        /// Creates a new RiotApi instance with the provided configuration.
        /// </summary>
        /// <param name="config">RiotApiConfig to use.</param>
        /// <returns>RiotApi instance using supplied configuration.</returns>
        public static RiotApi NewInstance(IRiotApiConfig config)
        {
            return new RiotApi(config);
        }

        #region requests
        public T Send<T>(Region region, string methodId, bool nonRateLimited,
            HttpRequestMessage request, CancellationToken? token)
        {
            return SendAsync<T>(region, methodId, nonRateLimited, request, token).Result;
        }

        public async Task<T> SendAsync<T>(Region region, string methodId, bool nonRateLimited,
            HttpRequestMessage request, CancellationToken? token)
        {
            // Camille's code is context-free.
            // This slightly improves performance and helps prevent GUI thread deadlocks.
            // https://blogs.msdn.microsoft.com/benwilli/2017/02/09/an-alternative-to-configureawaitfalse-everywhere/
            await new SynchronizationContextRemover();
            return await _requestManager.Send<T>(region, methodId, nonRateLimited, request, token);
        }
        #endregion
    }
}
