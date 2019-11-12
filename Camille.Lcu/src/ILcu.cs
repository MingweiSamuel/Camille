﻿using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Camille.Lcu
{
    public interface ILcu
    {
        /// <summary>
        /// Send a custom request to the LCU, parsing a value as JSON.
        /// </summary>
        /// <typeparam name="T">Type to parse as JSON.</typeparam>
        /// <param name="request">Request to send.</param>
        /// <param name="token">Cancellation token to cancel the request.</param>
        /// <returns>The parsed value. May be null if endpoint returned an empty success response.</returns>
        public T Send<T>(HttpRequestMessage request, CancellationToken? token);

        /// <summary>
        /// Send a custom request to the LCU, ignoring the return value.
        /// </summary>
        /// <typeparam name="T">Type to parse as JSON.</typeparam>
        /// <param name="request">Request to send.</param>
        /// <param name="token">Cancellation token to cancel the request.</param>
        public void Send(HttpRequestMessage request, CancellationToken? token);

        /// <summary>
        /// Send a custom request to the LCU, parsing a value as JSON.
        /// </summary>
        /// <typeparam name="T">Type to parse as JSON.</typeparam>
        /// <param name="request">Request to send.</param>
        /// <param name="token">Cancellation token to cancel the request.</param>
        /// <returns>The parsed value. May be null if endpoint returned an empty success response.</returns>
        public Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken? token);

        /// <summary>
        /// Send a custom request to the LCU, ignoring the return value.
        /// </summary>
        /// <typeparam name="T">Type to parse as JSON.</typeparam>
        /// <param name="request">Request to send.</param>
        /// <param name="token">Cancellation token to cancel the request.</param>
        public Task SendAsync(HttpRequestMessage request, CancellationToken? token);
    }
}
