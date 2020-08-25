using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Camille.RiotApi.Util;

#if USE_NEWTONSOFT
#elif USE_SYSTEXTJSON
#else
#error Must have one of USE_NEWTONSOFT or USE_SYSTEXTJSON set.
#endif

namespace Camille.RiotApi
{
    public class RiotApi : IRiotApi
    {
        /// <summary>RequestManager for sending web requests and managing rate limits.</summary>
        private readonly RequestManager _requestManager;


#if USE_NEWTONSOFT
        private static readonly CustomIntConverter customIntConverter = new CustomIntConverter();
        private class CustomIntConverter : Newtonsoft.Json.JsonConverter
        {
            public override bool CanConvert(Type objectType) { return typeof(int) == objectType; }

            public override object ReadJson(
                Newtonsoft.Json.JsonReader reader, Type objectType,
                object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (Newtonsoft.Json.JsonToken.Float == reader.TokenType)
                {
                    var doubleVal = serializer.Deserialize<double>(reader);
                    var intVal = (int) doubleVal;
                    if (doubleVal == intVal)
                        return intVal;
                    throw new Newtonsoft.Json.JsonException($"Cannot parse number as int: {doubleVal}.");
                }
                return (int) serializer.Deserialize<long>(reader);
            }

            public override bool CanWrite { get { return false; } }

            public override void WriteJson(
                Newtonsoft.Json.JsonWriter writer, object value,
                Newtonsoft.Json.JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
#elif USE_SYSTEXTJSON
        private static readonly System.Text.Json.JsonSerializerOptions jsonOptions = new System.Text.Json.JsonSerializerOptions();
        private class CustomIntConverter : System.Text.Json.Serialization.JsonConverter<int>
        {
            public override int Read(ref System.Text.Json.Utf8JsonReader reader, Type type, System.Text.Json.JsonSerializerOptions options)
            {
                var valDouble = reader.GetDouble();
                var valInt = (int) valDouble;
                if (valDouble == valInt)
                    return valInt;
                return reader.GetInt32();
            }

            public override void Write(System.Text.Json.Utf8JsonWriter writer, int value, System.Text.Json.JsonSerializerOptions options)
            {
                writer.WriteNumberValue(value);
            }
        }

        static RiotApi()
        {
            jsonOptions.Converters.Add(new CustomIntConverter());
        }
#endif


        private RiotApi(IRiotApiConfig config)
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

        public async Task<T> Send<T>(string route, string methodId, HttpRequestMessage request,
            CancellationToken? token = null, bool ignoreAppRateLimits = false)
        {
            // Camille's code is context-free.
            // This slightly improves performance and helps prevent GUI thread deadlocks.
            // https://blogs.msdn.microsoft.com/benwilli/2017/02/09/an-alternative-to-configureawaitfalse-everywhere/
            await new SynchronizationContextRemover();
            var content = await _requestManager.Send(route, methodId, request, token.GetValueOrDefault(), ignoreAppRateLimits);
            if (null == content) return default!; // TODO: throw exception on unexpected null.
            return Deserialize<T>(content);
        }

        public async Task Send(string route, string methodId, HttpRequestMessage request,
            CancellationToken? token = null, bool ignoreAppRateLimits = false)
        {
            await new SynchronizationContextRemover();
            await _requestManager.Send(route, methodId, request, token.GetValueOrDefault(), ignoreAppRateLimits);
        }

        // Visible for testing.
        internal static T Deserialize<T>(string content)
        {
#if USE_NEWTONSOFT
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content, customIntConverter);
#elif USE_SYSTEXTJSON
            return System.Text.Json.JsonSerializer.Deserialize<T>(content, jsonOptions);
#endif
        }
    }
}
