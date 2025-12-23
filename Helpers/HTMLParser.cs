using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Xml;

namespace DMAssistant.Helpers
{
    public static class HtmlParser
    {
        public static string HtmlToPlainText(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            string text = html;

            // Paragraphs → double newline
            text = Regex.Replace(text, @"<\s*p\s*>", "");
            text = Regex.Replace(text, @"<\s*/\s*p\s*>", "\n\n");

            // Emphasis → italic (could be extended later)
            text = Regex.Replace(text, @"<\s*em\s*>", "");
            text = Regex.Replace(text, @"<\s*/\s*em\s*>", "");

            // Strong → bold (could be extended later)
            text = Regex.Replace(text, @"<\s*strong\s*>", "");
            text = Regex.Replace(text, @"<\s*/\s*strong\s*>", "");

            // Line breaks
            text = Regex.Replace(text, @"<\s*br\s*/?\s*>", "\n");

            // Remove any other tags
            text = Regex.Replace(text, @"<[^>]+>", "");

            // Decode HTML entities
            text = WebUtility.HtmlDecode(text);

            return text.Trim();
        }

        public static FlowDocument HtmlToFlowDocument(string html)
        {
            var doc = new FlowDocument();

            if (string.IsNullOrWhiteSpace(html))
                return doc;

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            foreach (var node in htmlDoc.DocumentNode.ChildNodes)
            {
                if (node.Name == "p")
                {
                    var paragraph = new Paragraph();
                    AddInlines(paragraph.Inlines, node);
                    doc.Blocks.Add(paragraph);
                }
            }

            return doc;
        }
        private static void AddInlines(InlineCollection inlines, HtmlNode node)
        {
            foreach (var child in node.ChildNodes)
            {
                switch (child.Name)
                {
                    case "#text":
                        inlines.Add(new Run(child.InnerText));
                        break;

                    case "b":
                    case "strong":
                        var bold = new Bold();
                        AddInlines(bold.Inlines, child);
                        inlines.Add(bold);
                        break;

                    case "i":
                    case "em":
                        var italic = new Italic();
                        AddInlines(italic.Inlines, child);
                        inlines.Add(italic);
                        break;

                    case "br":
                        inlines.Add(new LineBreak());
                        break;
                    case "a":
                        var link = new Hyperlink
                        {
                            NavigateUri = new Uri(child.GetAttributeValue("href", "#"))
                        };
                        AddInlines(link.Inlines, child);
                        inlines.Add(link);
                        break;


                    default:
                        AddInlines(inlines, child);
                        break;
                }
            }
        }


    }
}
