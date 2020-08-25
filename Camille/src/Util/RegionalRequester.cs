using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MingweiSamuel.Camille.Enums;

namespace MingweiSamuel.Camille.Util
{
    /// <summary>
    /// Manages rate limits for a particular region and sends requests.
    /// Retries retryable responses (429, 5xx).
    /// Processes non-retryable responses (200, 404, 4xx).
    /// </summary>
    public class RegionalRequester
    {
        /// <summary>Root url for Riot API requests.</summary>
        private const string RiotRootUrl = ".api.riotgames.com";
        /// <summary>Request header name for the Riot API key.</summary>
        private const string RiotKeyHeader = "X-Riot-Token";

        /// <summary>Configuration information.</summary>
        private readonly IRiotApiConfig _config;

        /// <summary>Represents the app rate limit.</summary>
        private readonly IRateLimit _appRateLimit;

        /// <summary>Represents method rate limits.</summary>
        private readonly ConcurrentDictionary<string, IRateLimit> _methodRateLimits =
            new ConcurrentDictionary<string, IRateLimit>();

        /// <summary>
        /// HttpClient for sending requests. Better than using WebClient in this case.
        /// https://stackoverflow.com/a/27737601/2398020
        /// </summary>
        private readonly HttpClient _client = new HttpClient();

        public RegionalRequester(IRiotApiConfig config)
        {
            _config = config;
            _appRateLimit = new RateLimit(RateLimitType.Application, config);
        }

        /// <summary>
        /// HttpStatus codes that are considered a success, but will return null (or default(T)).
        /// Listed from most common to least common.
        /// </summary>
        private static readonly int[] NullSuccessStatusCodes = { 404, 204, 422 };

#nullable disable
        /// <summary>
        /// Sends a GET request, obeying rate limits and retry afters.
        /// </summary>
        /// <param name="methodId"></param>
        /// <param name="relativeUrl"></param>
        /// <param name="region"></param>
        /// <param name="queryParams"></param>
        /// <param name="nonRateLimited">If set to true, the request will not count against the application rate limit.</param>
        /// <param name="token">CancellationToken to cancel this task.</param>
        /// <returns></returns>
        public async Task<T> Get<T>(string methodId, string relativeUrl, Region region,
            IEnumerable<KeyValuePair<string, string>> queryParams, bool nonRateLimited, CancellationToken? token)
        {
            HttpResponseMessage response = null;
            var retries = 0;
            for (; retries <= _config.Retries; retries++)
            {
                // Get token.
                var methodRateLimit = GetMethodRateLimit(methodId);
                long delay;
                var rateLimits = nonRateLimited ? new[] { methodRateLimit } : new[] { _appRateLimit, methodRateLimit };
                while ((delay = RateLimitUtils.GetOrDelay(rateLimits)) >= 0)
                    await Task.Delay(TimeSpan.FromTicks(delay), token.GetValueOrDefault());

                // Send request.
                string query;
                using (var content = new FormUrlEncodedContent(queryParams))
                    query = await content.ReadAsStringAsync();

                var request = new HttpRequestMessage(HttpMethod.Get, $"https://{region.Platform}{RiotRootUrl}{relativeUrl}?{query}");
                request.Headers.Add(RiotKeyHeader, _config.ApiKey);

                // Receive response.
                response = await _client.SendAsync(request, token.GetValueOrDefault());
                foreach (var rateLimit in rateLimits)
                    rateLimit.OnResponse(response);

                // Success.
                if (HttpStatusCode.OK == response.StatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return Deserialize<T>(json);
                }
                if (0 <= Array.IndexOf(NullSuccessStatusCodes, (int) response.StatusCode))
                    return default;
                // Failure. 429 and 5xx are retryable. All else exit.
                if (429 == (int) response.StatusCode || response.StatusCode >= HttpStatusCode.InternalServerError)
                    continue;
                break;
            }
            throw new RiotResponseException(
                $"Request to {methodId} failed after {retries} retries. " +
                $"(status: {(int) (response?.StatusCode ?? 0)}).", response);
        }
#nullable restore

        private IRateLimit GetMethodRateLimit(string methodId)
        {
            return _methodRateLimits.GetOrAdd(methodId, m => new RateLimit(RateLimitType.Method, _config));
        }

        // Visible for testing.
        internal static T Deserialize<T>(string json)
        {
#if USE_NEWTONSOFT
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, customIntConverter);
#elif USE_SYSTEXTJSON
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, jsonOptions);
#endif
        }


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

        static RegionalRequester()
        {
            jsonOptions.Converters.Add(new CustomIntConverter());
        }
#endif
    }
}
