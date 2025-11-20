using System.Text.RegularExpressions;
using System.Net;

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
    }
}
