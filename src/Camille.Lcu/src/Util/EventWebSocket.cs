using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Camille.Lcu.Util
{
    public class EventWebSocket : IDisposable
    {
        private const int RECEIVE_CHUNK_SIZE = 1024;
        private const int SEND_CHUNK_SIZE = 1024;

        // TODO does reconnecting need a new _socket?
        private readonly ClientWebSocket _socket = new ClientWebSocket();
        private readonly Uri _uri;

        public event Func<Task?>? OnConnect;
        public event Func<byte[], Task?>? OnMessage;
        public event Func<Task?>? OnDisconnect;

        private volatile Task? _eventLoopTask;
        private readonly CancellationTokenSource _eventLoopCancellation = new CancellationTokenSource();

        public EventWebSocket(Uri uri, ICredentials credentials, RemoteCertificateValidationCallback validationCallback)
        {
            _uri = uri;

            _socket.Options.AddSubProtocol("wamp");
            _socket.Options.Credentials = credentials;
            _socket.Options.RemoteCertificateValidationCallback = validationCallback;
        }

        private async Task EventLoop()
        {
            var token = _eventLoopCancellation.Token;

            var buffer = new byte[RECEIVE_CHUNK_SIZE];
            var segment = new ArraySegment<byte>(buffer);
            var memoryStream = new MemoryStream();

            // Connect socket.
            await _socket.ConnectAsync(_uri, token);

            // Run event loop.
            try
            {
                await (OnConnect?.Invoke() ?? Task.CompletedTask);

                // Receive messages.
                while (WebSocketState.Open == _socket.State)
                {
                    WebSocketReceiveResult received;
                    do
                    {
                        received = await _socket.ReceiveAsync(segment, token);
                        memoryStream.Write(buffer, 0, received.Count); // Don't use async, memoryStream doesn't do IO.
                    }
                    while (!received.EndOfMessage);

                    // Trigger event. ToArray makes copy of buffer.
                    await (OnMessage?.Invoke(memoryStream.ToArray()) ?? Task.CompletedTask);

                    // Reset memoryStream (also sets the position to zero).
                    memoryStream.SetLength(0);
                }
            }
            finally
            {
                // Trigger OnDisconnect.
                try
                {
                    await (OnDisconnect?.Invoke() ?? Task.CompletedTask);
                }
                finally
                {
                    // Close Socket.
                    try
                    {
                        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token);
                    }
                    catch
                    {
                        _socket.Abort();
                    }
                    finally
                    {
                        _socket.Dispose();
                    }
                }
            }
        }

        public void Connect()
        {
            lock (this)
            {
                if (null != _eventLoopTask)
                    throw new InvalidOperationException($"Cannot Connect {nameof(EventWebSocket)} twice.");
                _eventLoopTask = EventLoop();
            }
        }

        public async Task SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType msgType, bool end, CancellationToken? token = null)
        {
            if (WebSocketState.Open != _socket.State)
                throw new InvalidOperationException($"Cannot send to socket that is not open.");
            await _socket.SendAsync(buffer, msgType, end, token.GetValueOrDefault());
        }

        public async Task Close(CancellationToken? token = null)
        {
            lock (this)
            {
                if (null == _eventLoopTask)
                    throw new InvalidOperationException($"Cannot Close {nameof(EventWebSocket)} before calling Connect.");
            }

            _eventLoopCancellation.Cancel();
            try
            {
                await _eventLoopTask;
                await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token.GetValueOrDefault());
            }
            catch (OperationCanceledException)
            {}
        }

        public void Dispose()
        {
            _eventLoopCancellation.Cancel();
        }
    }
}
