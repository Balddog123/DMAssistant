using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Documents;
using System.Windows.Markup;

namespace DMAssistant.Helpers
{
    public sealed class FlowDocumentJsonConverter : JsonConverter<FlowDocument>
    {
        public override FlowDocument Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                return new FlowDocument();

            string value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value)) return new FlowDocument();

            // Case 1: Old version → plain text
            if (!value.TrimStart().StartsWith("<FlowDocument", StringComparison.Ordinal))
            {
                var doc = HtmlParser.HtmlToFlowDocument(value);
                //doc.Blocks.Add(new Paragraph(new Run(value)));
                return doc;
            }

            // Case 2: New version → XAML FlowDocument
            try
            {
                return (FlowDocument)XamlReader.Parse(value);
            }
            catch
            {
                // Fail safely — never crash deserialization
                var fallback = new FlowDocument();
                fallback.Blocks.Add(new Paragraph(new Run(value)));
                return fallback;
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            FlowDocument value,
            JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            string xaml = XamlWriter.Save(value);
            writer.WriteStringValue(xaml);
        }
    }
}
