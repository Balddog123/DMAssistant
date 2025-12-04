using DMAssistant.Model;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DMAssistant.Helpers
{
    public class SafeEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? enumString = reader.GetString();

                if (Enum.TryParse(enumString, ignoreCase: true, out T value))
                    return value;

                // Log + fallback
                Debug.WriteLine($"[SafeEnumConverter] Unknown enum value '{enumString}' for {typeof(T).Name}");
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt32(out int enumInt))
                {
                    if (Enum.IsDefined(typeof(T), enumInt))
                        return (T)Enum.ToObject(typeof(T), enumInt);
                }

                Debug.WriteLine($"[SafeEnumConverter] Unknown enum numeric value '{reader.GetInt32()}' for {typeof(T).Name}");
            }

            // Fallback: return first enum value
            return default!;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

}
