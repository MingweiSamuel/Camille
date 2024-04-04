using System;

namespace Camille.Core
{
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
}
