using Camille.Lcu.Util;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Camille.Core;

namespace Camille.Lcu
{
    public class LcuApi : ILcuApi, IDisposable
    {
        private readonly LcuRequester _requester;
        public readonly Wamp wamp;

        public LcuApi(Lockfile lockfile) : this(lockfile, new LcuConfig())
        {}

        public LcuApi(Lockfile lockfile, LcuConfig config) : base()
        {
            _requester = new LcuRequester(lockfile, config);
            wamp = new Wamp(lockfile, config);
        }

        /// <inheritdoc/>
        public async Task<T> Send<T>(HttpRequestMessage request, CancellationToken? token = null)
        {
            // Camille's code is context-free.
            // This slightly improves performance and helps prevent GUI thread deadlocks.
            // https://blogs.msdn.microsoft.com/benwilli/2017/02/09/an-alternative-to-configureawaitfalse-everywhere/
            await new SynchronizationContextRemover();
            var content = await _requester.SendAsync(request, token.GetValueOrDefault());
            if (null == content) return default!; // TODO: throw exception on unexpected null.
            return JsonHandler.Deserialize<T>(content);
        }

        /// <inheritdoc/>
        public async Task Send(HttpRequestMessage request, CancellationToken? token = null)
        {
            await new SynchronizationContextRemover();
            await _requester.SendAsync(request, token.GetValueOrDefault());
        }

        public void Dispose()
        {
            _requester.Dispose();
            wamp.Dispose();
        }
    }
}
