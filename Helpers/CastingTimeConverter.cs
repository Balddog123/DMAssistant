using DMAssistant.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DMAssistant.Helpers
{
    public class CastingTimeConverter : JsonConverter<CastingTime>
    {
        public override CastingTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString()?.ToLowerInvariant();

            return value switch
            {
                "1 action" => CastingTime.Action,
                "action" => CastingTime.Action,
                "1 bonus action" => CastingTime.BonusAction,
                "bonus action" => CastingTime.BonusAction,
                "reaction" => CastingTime.Reaction,
                "1 reaction" => CastingTime.Reaction,
                _ => throw new JsonException($"Unknown casting time: {value}")
            };
        }

        public override void Write(Utf8JsonWriter writer, CastingTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

}
