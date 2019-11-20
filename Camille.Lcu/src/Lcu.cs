using Camille.Lcu.src;
using Camille.Lcu.Util;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Camille.Lcu
{
    public class Lcu : ILcu, IDisposable
    {
        private readonly LcuRequester _requester;
        private readonly LcuWamp _wamp;

        public Lcu(Lockfile lockfile) : this(lockfile, new LcuConfig())
        {}

        public Lcu(Lockfile lockfile, LcuConfig config) : base()
        {
            _requester = new LcuRequester(lockfile, config);
            _wamp = new LcuWamp(lockfile, config);
        }

        public async Task Connect(CancellationToken? token = null)
        {
            await _wamp.Connect(token);
        }

        /// <inheritdoc/>
        public async Task<T> Send<T>(HttpRequestMessage request, CancellationToken? token = null)
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

        /// <inheritdoc/>
        public async Task Send(HttpRequestMessage request, CancellationToken? token = null)
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
