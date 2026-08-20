using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Camille.Enums
{
    /// <summary>
    /// Deserializes <see cref="QueueType"/>. An unrecognized queueType string (e.g. a newly added
    /// or revived ranked queue not yet reflected in queueTypes.json) falls back to
    /// <see cref="QueueType.UNKNOWN"/> instead of throwing, so it doesn't fail an entire response
    /// array over one entry. Known values still round-trip through their enum member name.
    /// </summary>
    public class QueueTypeConverter : JsonConverter<QueueType>
    {
        public override QueueType Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                return QueueType.UNKNOWN;
            }

            var value = reader.GetString()!;
            var isParsed = Enum.TryParse<QueueType>(value, ignoreCase: true, out var queueType) 
                        && Enum.IsDefined(typeof(QueueType), queueType);

            return isParsed ? queueType : QueueType.UNKNOWN;
        }

        public override void Write(Utf8JsonWriter writer, QueueType value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}
