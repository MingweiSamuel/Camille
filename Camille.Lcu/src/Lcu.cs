using Camille.Lcu.Util;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Camille.Lcu
{
    public class Lcu : IDisposable
    {
        private readonly LcuRequester _requester;

        public Lcu() : this(new LcuConfig()) {}

        public Lcu(LcuConfig lcuConfig)
        {
            _requester = new LcuRequester(lcuConfig);
        }

        /// <summary>
        /// Send a custom request to the LCU, parsing a value as JSON.
        /// </summary>
        /// <typeparam name="T">Type to parse as JSON.</typeparam>
        /// <param name="request">Request to send.</param>
        /// <param name="token">Cancellation token to cancel the request.</param>
        /// <returns>The parsed value. May be null if endpoint returned an empty success response.</returns>
        public T Send<T>(HttpRequestMessage request, CancellationToken? token)
        {
            return SendAsync<T>(request, token).Result;
        }

        /// <summary>
        /// Send a custom request to the LCU, ignoring the return value.
        /// </summary>
        /// <typeparam name="T">Type to parse as JSON.</typeparam>
        /// <param name="request">Request to send.</param>
        /// <param name="token">Cancellation token to cancel the request.</param>
        public void Send(HttpRequestMessage request, CancellationToken? token)
        {
            SendAsync(request, token).Wait();
        }


        /// <summary>
        /// Send a custom request to the LCU, parsing a value as JSON.
        /// </summary>
        /// <typeparam name="T">Type to parse as JSON.</typeparam>
        /// <param name="request">Request to send.</param>
        /// <param name="token">Cancellation token to cancel the request.</param>
        /// <returns>The parsed value. May be null if endpoint returned an empty success response.</returns>
        public async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken? token)
        {
            // Camille's code is context-free.
            // This slightly improves performance and helps prevent GUI thread deadlocks.
            // https://blogs.msdn.microsoft.com/benwilli/2017/02/09/an-alternative-to-configureawaitfalse-everywhere/
            await new SynchronizationContextRemover();
            var content = await _requester.SendAsync(request, token.GetValueOrDefault());
#if USE_NEWTONSOFT
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
#endif
#if USE_SYSTEXTJSON
            return System.Text.Json.JsonSerializer.Deserialize<T>(content);
#endif
        }

        /// <summary>
        /// Send a custom request to the LCU, ignoring the return value.
        /// </summary>
        /// <typeparam name="T">Type to parse as JSON.</typeparam>
        /// <param name="request">Request to send.</param>
        /// <param name="token">Cancellation token to cancel the request.</param>
        public async Task SendAsync(HttpRequestMessage request, CancellationToken? token)
        {
            await new SynchronizationContextRemover();
            await _requester.SendAsync(request, token.GetValueOrDefault());
        }

        public void Dispose()
        {
            _requester.Dispose();
        }
    }
}
