using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Camille.LolGame
{
    public interface ILolGameApi
    {
        /// <summary>
        /// Send a custom request to the LoL Game Client API, parsing the response as JSON.
        /// </summary>
        /// <typeparam name="T">Type to parse as JSON.</typeparam>
        /// <param name="request">Request to send.</param>
        /// <param name="token">Cancellation token to cancel the request.</param>
        /// <returns>The parsed value. May be null if endpoint returned an empty success response.</returns>
        public Task<T> Send<T>(HttpRequestMessage request, CancellationToken? token = null);

        /// <summary>
        /// Send a custom request to the LoL Game Client API, ignoring the return value.
        /// </summary>
        /// <typeparam name="T">Type to parse as JSON.</typeparam>
        /// <param name="request">Request to send.</param>
        /// <param name="token">Cancellation token to cancel the request.</param>
        public Task Send(HttpRequestMessage request, CancellationToken? token = null);
    }
}
