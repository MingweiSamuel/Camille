using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if USE_NEWTONSOFT
using JsonToken = Newtonsoft.Json.Linq.JToken;
#elif USE_SYSTEXTJSON
using JsonToken = System.Text.Json.JsonElement;
#else
#error One of USE_NEWTONSOFT or USE_SYSTEXTJSON must be defined.
#endif

namespace Camille.Lcu.Util
{
    /// <summary>
    /// For interacting with the Web Application Messaging Protocol (WAMP) pub-sub standard.
    ///
    /// This class is UNSTABLE and subject to change.
    /// </summary>
    public class Wamp : IDisposable
    {
        private readonly ImmutableHashSet<Func<JsonToken, Task<HandlerResult>>> EMPTY =
            ImmutableHashSet.Create<Func<JsonToken, Task<HandlerResult>>>();
        private readonly ConcurrentDictionary<string, ImmutableHashSet<Func<JsonToken, Task<HandlerResult>>>> handlers =
            new ConcurrentDictionary<string, ImmutableHashSet<Func<JsonToken, Task<HandlerResult>>>>();

        private readonly LcuConfig _config;
        private readonly Lockfile _lockfile;
        private readonly EventWebSocket _eventSocket;

        public event Func<Task?>? OnConnect;
        public event Func<Task?>? OnDisconnect;

        public Wamp(Lockfile lockfile, LcuConfig config)
        {
            _config = config;
            _lockfile = lockfile;

            _eventSocket = new EventWebSocket(
                new UriBuilder("wss", "127.0.0.1", lockfile.Port).Uri,
                new NetworkCredential("riot", _lockfile.Password),
                _config.CertificateValidationCallback);

            _eventSocket.OnConnect += HandleConnect;
            _eventSocket.OnMessage += HandleMessage;
            _eventSocket.OnDisconnect += HandleDisconnect;
        }

        private Task? HandleConnect()
        {
            return OnConnect?.Invoke();
        }

        private Task? HandleDisconnect()
        {
            return OnDisconnect?.Invoke();
        }

        private async Task HandleMessage(byte[] buffer)
        {
            JsonToken[] message;
            WampMessageType messageType;

#if USE_NEWTONSOFT
            message = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonToken[]>(Encoding.UTF8.GetString(buffer));
            messageType = (WampMessageType) message[0].ToObject<int>();
#elif USE_SYSTEXTJSON
            message = System.Text.Json.JsonSerializer.Deserialize<JsonToken[]>(buffer)
                ?? throw new InvalidOperationException("Received null message.");
            messageType = (WampMessageType) message[0].GetInt32();
#endif

            switch (messageType)
            {
                case WampMessageType.Welcome: // session, protocolVersion, details.
                    break;
                case WampMessageType.Event: // topic, payload.
                    string topic;
                    JsonToken payload = message[2];
#if USE_NEWTONSOFT
                    topic = message[1].ToObject<string>();
#elif USE_SYSTEXTJSON
                    topic = message[1].GetString() ?? throw new InvalidOperationException("Topic is null.");
#endif
                    //Console.WriteLine($"Event: {topic} {payload}.");

                    if (handlers.TryGetValue(topic, out var topicHandlers))
                    {
                        foreach (var handler in topicHandlers)
                            await (handler.Invoke(payload) ?? Task.CompletedTask);
                    }
                    break;
                default:
                    //Console.WriteLine("Unhandled message type: " + messageType);
                    //Console.WriteLine('[' + string.Join(", ", message.Select(m => m.ToString())) + ']');
                    break;
            }
        }

        public void Connect()
        {
            _eventSocket.Connect();
        }

        public async Task Subscribe(string topic, Func<JsonToken, Task<HandlerResult>> handler, CancellationToken? token = null)
        {
            handlers.GetOrAdd(topic, (_topic) => ImmutableHashSet.Create(handler));

            var data = new object[] { WampMessageType.Subscribe, topic };
#if USE_NEWTONSOFT
            var message = Newtonsoft.Json.JsonConvert.SerializeObject(data);
#elif USE_SYSTEXTJSON
            var message = System.Text.Json.JsonSerializer.Serialize(data);
#endif
            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await _eventSocket.SendAsync(buffer, System.Net.WebSockets.WebSocketMessageType.Text, true, token);
        }

        public void Unsubscribe(string topic, Func<JsonToken, Task<HandlerResult>> handler)
        {
            handlers.AddOrUpdate(topic, EMPTY, (_topic, set) => set.Remove(handler));
        }

        public async Task Close(CancellationToken? token = null)
        {
            await _eventSocket.Close(token);
        }

        public void Dispose()
        {
            _eventSocket.Dispose();
        }
    }
}
