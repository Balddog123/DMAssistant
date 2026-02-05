using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DMAssistant.Helpers
{
    public static class InlineDebugger
    {
        // Entry point
        public static void DebugPrintFlowDocument(FlowDocument doc)
        {
            foreach (var block in doc.Blocks)
                DebugPrintBlock(block);
        }

        private static void DebugPrintBlock(Block block)
        {
            switch (block)
            {
                case Paragraph para:
                    Debug.WriteLine("Paragraph:");
                    foreach (var inline in para.Inlines)
                        DebugPrintInline(inline, "  ");
                    break;

                case Section section:
                    Debug.WriteLine("Section:");
                    foreach (var child in section.Blocks)
                        DebugPrintBlock(child);
                    break;

                case List list:
                    Debug.WriteLine("List:");
                    foreach (var item in list.ListItems)
                    {
                        Debug.WriteLine(" ListItem:");
                        foreach (var child in item.Blocks)
                            DebugPrintBlock(child);
                    }
                    break;

                default:
                    Debug.WriteLine($"Unknown block type: {block.GetType()}");
                    break;
            }
        }

        private static void DebugPrintInline(Inline inline, string indent)
        {
            switch (inline)
            {
                case Run run:
                    Debug.WriteLine($"{indent}Run: '{run.Text}'");
                    break;

                case Bold bold:
                    Debug.WriteLine($"{indent}Bold:");
                    foreach (var child in bold.Inlines)
                        DebugPrintInline(child, indent + "  ");
                    break;

                case Italic italic:
                    Debug.WriteLine($"{indent}Italic:");
                    foreach (var child in italic.Inlines)
                        DebugPrintInline(child, indent + "  ");
                    break;

                case Underline underline:
                    Debug.WriteLine($"{indent}Underline:");
                    foreach (var child in underline.Inlines)
                        DebugPrintInline(child, indent + "  ");
                    break;

                case Hyperlink link:
                    string tagInfo = link.Tag != null ? link.Tag.ToString() : "null";
                    Debug.WriteLine($"{indent}Hyperlink (Tag={tagInfo}):");
                    foreach (var child in link.Inlines)
                        DebugPrintInline(child, indent + "  ");
                    break;

                case Span span:
                    Debug.WriteLine($"{indent}Span:");
                    foreach (var child in span.Inlines)
                        DebugPrintInline(child, indent + "  ");
                    break;

                default:
                    Debug.WriteLine($"{indent}Unknown inline type: {inline.GetType()}");
                    break;
            }
        }
    }
}
