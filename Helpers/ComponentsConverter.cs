using DMAssistant.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DMAssistant.Helpers
{
    public class ComponentsConverter : JsonConverter<Components>
    {
        public override Components Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            return new Components
            {
                material = root.GetProperty("material").GetBoolean(),
                somatic = root.GetProperty("somatic").GetBoolean(),
                verbal = root.GetProperty("verbal").GetBoolean(),
                raw = root.TryGetProperty("raw", out var rawVal)
                        ? rawVal.GetString()!.Split(',').Select(s => s.Trim()).ToList()
                        : new List<string>()
            };
        }

        public override void Write(Utf8JsonWriter writer, Components value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteBoolean("material", value.material);
            writer.WriteBoolean("somatic", value.somatic);
            writer.WriteBoolean("verbal", value.verbal);
            writer.WriteString("raw", string.Join(", ", value.raw));
            writer.WriteEndObject();
        }
    }

}
