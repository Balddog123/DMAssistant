using DMAssistant.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DMAssistant.Helpers
{
    public class SchoolConverter : JsonConverter<Spell.SchoolOfMagic>
    {
        public override Spell.SchoolOfMagic Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (Enum.TryParse<Spell.SchoolOfMagic>(value, true, out var result))
                return result;

            throw new JsonException($"Unknown school: {value}");
        }

        public override void Write(Utf8JsonWriter writer, Spell.SchoolOfMagic value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

}
