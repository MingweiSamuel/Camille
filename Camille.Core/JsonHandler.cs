using System;

namespace Camille.Core
{
#if USE_NEWTONSOFT
    public static class JsonHandler
    {
        /// <summary>Settings singleton.</summary>
        private static readonly Newtonsoft.Json.JsonSerializerSettings jsonSerializerSettings =
            new Newtonsoft.Json.JsonSerializerSettings()
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                Converters = new[] { new CustomIntConverter() },
            };

        /// <summary>Int32 converter which is able to read integers with decimal points "123.0".</summary>
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

        public static T Deserialize<T>(string content)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content, jsonSerializerSettings);
        }

        public static string Serialize<T>(T value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value, jsonSerializerSettings);
        }
    }
#elif USE_SYSTEXTJSON
    public static class JsonHandler
    {
        /// <summary>Options singleton.</summary>
        private static readonly System.Text.Json.JsonSerializerOptions jsonOptions =
            new System.Text.Json.JsonSerializerOptions()
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            };

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

        static JsonHandler()
        {
            jsonOptions.Converters.Add(new CustomIntConverter());
        }

        public static T Deserialize<T>(string content)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(content, jsonOptions)
                ?? throw new ArgumentException("Deserialized JSON content is null.");
        }

        public static string Serialize<T>(T value)
        {
            return System.Text.Json.JsonSerializer.Serialize(value, jsonOptions);
        }
    }
#else
#error One of USE_NEWTONSOFT or USE_SYSTEXTJSON must be set.
#endif
}
