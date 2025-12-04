using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DMAssistant.Helpers
{
    public class LocationJsonConverter : JsonConverter<Location?>
    {
        public override Location? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Case 1: null
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            // Case 2: legacy string format
            if (reader.TokenType == JsonTokenType.String)
            {
                string name = reader.GetString();
                if (string.IsNullOrWhiteSpace(name))
                    return null;

                return ResolveLocationByName(name);
            }

            // Case 3: object → new format
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Clone options but remove THIS converter to avoid recursion
                var noSelfOptions = new JsonSerializerOptions(options);
                var thisConverter = noSelfOptions.Converters.FirstOrDefault(c => c is LocationJsonConverter);
                if (thisConverter != null)
                    noSelfOptions.Converters.Remove(thisConverter);

                // Now it's safe — no recursion
                return JsonSerializer.Deserialize<Location>(ref reader, noSelfOptions);
            }

            throw new JsonException($"Unexpected token {reader.TokenType} when parsing Location");
        }

        public override void Write(Utf8JsonWriter writer, Location? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            // Serialize with same strategy: clone options without this converter
            var noSelfOptions = new JsonSerializerOptions(options);
            var thisConverter = noSelfOptions.Converters.FirstOrDefault(c => c is LocationJsonConverter);
            if (thisConverter != null)
                noSelfOptions.Converters.Remove(thisConverter);

            JsonSerializer.Serialize(writer, value, noSelfOptions);
        }

        private Location? ResolveLocationByName(string name)
        {
            // Adapt this to your actual storage
            // For example:
            foreach(Location location in App.CampaignStore.CurrentCampaign.Locations)
            {
                if (location.Name == name) return location;
            }
            return null;
        }
    }
}
