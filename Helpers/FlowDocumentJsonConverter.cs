using DMAssistant.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml.Linq;

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
            if (!value.TrimStart().StartsWith("<", StringComparison.Ordinal))
            {
                var doc = HtmlParser.HtmlToFlowDocument(value);
                //doc.Blocks.Add(new Paragraph(new Run(value)));
                return doc;
            }

            // Case 2: New version → XAML FlowDocument
            try
            {
                return DeserializeFlowDocument(value);
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

            string xml = SerializeFlowDocument(value);
            writer.WriteStringValue(xml);
        }

        //Serialization
        string SerializeFlowDocument(FlowDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var block in doc.Blocks)
                SerializeBlock(block, sb);

            return sb.ToString();
        }
        void SerializeBlock(Block block, StringBuilder sb)
        {
            switch (block)
            {
                case Paragraph para:
                    sb.Append("<p>");
                    foreach (var inline in para.Inlines)
                        SerializeInline(inline, sb);
                    sb.Append("</p>");
                    break;

                case Section section:
                    foreach (var child in section.Blocks)
                        SerializeBlock(child, sb);
                    break;

                case List list:
                    foreach (var item in list.ListItems)
                        foreach (var child in item.Blocks)
                            SerializeBlock(child, sb);
                    break;
            }
        }
        void SerializeInline(Inline inline, StringBuilder sb)
        {
            switch (inline)
            {
                case Run run:
                    sb.Append(System.Security.SecurityElement.Escape(run.Text));
                    break;

                case Bold bold:
                    sb.Append("<b>");
                    foreach (var child in bold.Inlines)
                        SerializeInline(child, sb);
                    sb.Append("</b>");
                    break;

                case Italic italic:
                    sb.Append("<i>");
                    foreach (var child in italic.Inlines)
                        SerializeInline(child, sb);
                    sb.Append("</i>");
                    break;

                case Underline underline:
                    sb.Append("<u>");
                    foreach (var child in underline.Inlines)
                        SerializeInline(child, sb);
                    sb.Append("</u>");
                    break;

                case Hyperlink link:

                    Debug.WriteLine($"HYPERLINK: {link.Tag}");
                    string key = HyperlinkMetadata.GetLinkKey(link);
                    LinkType type = HyperlinkMetadata.GetLinkType(link);

                    sb.Append($"<l key=\"{System.Security.SecurityElement.Escape(key)}\" type=\"{type.ToString()}\">");
                    foreach (var child in link.Inlines)
                        SerializeInline(child, sb);
                    sb.Append("</l>");
                    break;

                case Span span:
                    foreach (var child in span.Inlines)
                        SerializeInline(child, sb);
                    break;
            }
        }

        //Deserialization
        FlowDocument DeserializeFlowDocument(string xml)
        {
            FlowDocument doc = new FlowDocument();

            var xdoc = XDocument.Parse($"<root>{xml}</root>"); // wrap in root for multiple paragraphs
            foreach (var node in xdoc.Root.Elements())
                doc.Blocks.Add(ParseBlock(node));
            return doc;
        }
        Block ParseBlock(XElement elem)
        {
            if (elem.Name == "p")
            {
                Paragraph para = new Paragraph();
                foreach (var child in elem.Nodes())
                    ParseInline(child, para.Inlines);
                return para;
            }

            throw new NotImplementedException($"Unknown block {elem.Name}");
        }

        void ParseInline(XNode node, InlineCollection inlines)
        {
            switch (node)
            {
                case XText textNode:
                    inlines.Add(new Run(textNode.Value));
                    break;

                case XElement elem:
                    InlineCollection targetCollection = inlines;

                    switch (elem.Name.LocalName)
                    {
                        case "b":
                            Bold bold = new Bold();
                            foreach (var child in elem.Nodes())
                                ParseInline(child, bold.Inlines);
                            inlines.Add(bold);
                            break;

                        case "i":
                            Italic italic = new Italic();
                            foreach (var child in elem.Nodes())
                                ParseInline(child, italic.Inlines);
                            inlines.Add(italic);
                            break;

                        case "u":
                            Underline underline = new Underline();
                            foreach (var child in elem.Nodes())
                                ParseInline(child, underline.Inlines);
                            inlines.Add(underline);
                            break;

                        case "l":
                            string key = elem.Attribute("key")?.Value ?? "";
                            string type = elem.Attribute("type")?.Value ?? "";

                            
                            if(Enum.TryParse(type, true, out LinkType newType))
                            {
                                //object parsedObject = HyperlinkHelper.GetObject(newType, key);

                                Hyperlink link = new Hyperlink(new Run(key))
                                {
                                    Name = Regex.Replace(key, @"\s+", "")
                                };
                                HyperlinkMetadata.SetLinkType(link, newType);
                                HyperlinkMetadata.SetLinkKey(link, key);
                                link.TextDecorations = TextDecorations.Underline;
                                Debug.WriteLine($"Found hyperlink {link.Name} with tag: {key} of type {type}");
                                Debug.WriteLine($"New Tag: {HyperlinkMetadata.GetLinkKey(link)}");
                                inlines.Add(link);
                            }
                            else
                            {
                                Debug.WriteLine($"Couldn't parse out type");

                            }


                            //foreach (var child in elem.Nodes())
                            //        ParseInline(child, link.Inlines);

                            
                            break;

                        default:
                            // unknown element → just parse children recursively
                            foreach (var child in elem.Nodes())
                                ParseInline(child, inlines);
                            break;
                    }

                    break;
            }
        }

    }
}
