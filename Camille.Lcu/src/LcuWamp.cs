using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Camille.Lcu.src
{
    class LcuWamp : IDisposable
    {
        private const string WEBSOCKET_PROTOCOL = "wss";

        private readonly ClientWebSocket _client = new ClientWebSocket();

        private readonly LcuConfig _config;
        private readonly Lockfile _lockfile;
        private readonly Uri _uri;

        public LcuWamp(Lockfile lockfile, LcuConfig config)
        {
            _config = config;
            _lockfile = lockfile;
            _uri = new UriBuilder(WEBSOCKET_PROTOCOL, "127.0.0.1", lockfile.Port).Uri;
        }

        public async Task Connect(CancellationToken? token = null)
        {
            var t = token.GetValueOrDefault();

            _client.Options.AddSubProtocol("wamp");
            _client.Options.Credentials = new NetworkCredential("riot", _lockfile.Password);
            Console.WriteLine(_lockfile.Password);
#if USE_REMOTECERTIFICATEVALIDATIONCALLBACK
            _client.Options.RemoteCertificateValidationCallback = _config.CertificateValidationCallback;
#endif
            await _client.ConnectAsync(_uri, t);

            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes("[5, \"OnJsonApiEvent\"]"));
            await _client.SendAsync(buffer, WebSocketMessageType.Text, true, t);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
