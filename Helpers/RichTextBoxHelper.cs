using CommunityToolkit.Mvvm.DependencyInjection;
using DMAssistant.Model;
using DMAssistant.View;
using DMAssistant.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Primitives;

namespace DMAssistant.Helpers
{
    public static class RichTextBoxHelper
    {
        public static readonly DependencyProperty BoundDocumentProperty =
            DependencyProperty.RegisterAttached(
                "BoundDocument",
                typeof(FlowDocument),
                typeof(RichTextBoxHelper),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnBoundDocumentChanged));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached(
                "IsUpdating",
                typeof(bool),
                typeof(RichTextBoxHelper),
                new PropertyMetadata(false));

        public static FlowDocument GetBoundDocument(DependencyObject obj)
            => (FlowDocument)obj.GetValue(BoundDocumentProperty);

        public static void SetBoundDocument(DependencyObject obj, FlowDocument value)
            => obj.SetValue(BoundDocumentProperty, value);

        private static bool GetIsUpdating(DependencyObject obj)
            => (bool)obj.GetValue(IsUpdatingProperty);

        private static void SetIsUpdating(DependencyObject obj, bool value)
            => obj.SetValue(IsUpdatingProperty, value);

        private static void OnBoundDocumentChanged( DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not RichTextBox rtb || GetIsUpdating(rtb))
                return;


            rtb.TextChanged -= RichTextBox_TextChanged;

            try
            {
                SetIsUpdating(rtb, true);

                if (e.NewValue is FlowDocument newDoc)
                {
                    rtb.Document = CloneDocument(newDoc);
                }
                else
                {
                    rtb.Document = new FlowDocument();
                }
            }
            finally
            {
                SetIsUpdating(rtb, false);
            }

            rtb.TextChanged += RichTextBox_TextChanged;
            rtb.PreviewMouseLeftButtonDown += RichTextBox_PreviewMouseLeftButtonDown;
        }


        private static void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not RichTextBox rtb)
                return;

            if (GetIsUpdating(rtb))
                return;

            try
            {
                SetIsUpdating(rtb, true);

                DetectTokenAtCaret(rtb.Document);

                FlowDocument currentDoc = CloneDocument(rtb.Document);
                //FlowDocument currentDoc = rtb.Document;
                SetBoundDocument(rtb, currentDoc);

                InlineDebugger.DebugPrintFlowDocument(currentDoc);
            }
            finally
            {
                SetIsUpdating(rtb, false);
            }
        }
        private static void DetectTokenAtCaret(FlowDocument document)
        {
            if (document == null)
                return;

            var rtb = Keyboard.FocusedElement as RichTextBox;
            if (rtb == null)
                return;

            TextPointer caret = rtb.CaretPosition;
            Paragraph? paragraph = caret.Paragraph;
            if (paragraph == null)
                return;

            string paragraphText = new TextRange(paragraph.ContentStart, paragraph.ContentEnd).Text;
            int offset = new TextRange(paragraph.ContentStart, caret).Text.Length;
            string textBeforeCaret = paragraphText.Substring(0, offset);

            var match = Regex.Match(textBeforeCaret, @"\{[simlc]:[^}]+\}$");
            if (!match.Success) return;

            TextPointer start = paragraph.ContentStart;
            TextPointer? tokenStart = GetTextPointerAtOffset(start, match.Index);
            TextPointer? tokenEnd = GetTextPointerAtOffset(start, match.Index + match.Length);

            if (tokenStart == null || tokenEnd == null) return;

            TextRange range = new TextRange(tokenStart, tokenEnd);

            string tokenText = range.Text;
            char type = tokenText[1];
            string key = tokenText.Substring(3, tokenText.Length - 4);

            Object objectForLink = null;
            string newText = "";
            LinkType linkType = LinkType.None;

            if (type == 's')
            {
                linkType = LinkType.Spell;
                var spellDict = App.CampaignStore.CurrentCampaign.Spells.ToDictionary(s => s.Name.ToLower().Trim(), s => s);
                if (spellDict.TryGetValue(key.ToLower().Trim(), out var spell))
                {
                    objectForLink = spell;
                    newText = spell.Name;
                }
                else return;
            }
            else if (type == 'i')
            {
                linkType = LinkType.Item;
                var itemDict = App.CampaignStore.CurrentCampaign.Items.ToDictionary(i => i.Name.ToLower().Trim(), i => i);
                if (itemDict.TryGetValue(key.ToLower().Trim(), out var item))
                {
                    objectForLink = item;
                    newText = item.Name;
                }
                else return;
            }
            else if (type == 'm')
            {
                linkType = LinkType.Monster;
                var monsterDict = App.CampaignStore.CurrentCampaign.Monsters.ToDictionary(m => m.Name.ToLower().Trim(), m => m);
                if (monsterDict.TryGetValue(key.ToLower().Trim(), out var monster))
                {
                    objectForLink = monster;
                    newText = monster.Name;
                }
                else return;
            }
            else if (type == 'l')
            {
                linkType = LinkType.Location;
                var locationDict = App.CampaignStore.CurrentCampaign.Locations.ToDictionary(l => l.Name.ToLower().Trim(), l => l);
                if (locationDict.TryGetValue(key.ToLower().Trim(), out var location))
                {
                    objectForLink = location;
                    newText = location.Name;
                }
                else return;
            }
            else if (type == 'c')
            {
                linkType = LinkType.NPC;
                var dict = App.CampaignStore.CurrentCampaign.NPCs.ToDictionary(n => n.Name.ToLower().Trim(), n => n);
                if (dict.TryGetValue(key.ToLower().Trim(), out var npc))
                {
                    objectForLink = npc;
                    newText = npc.Name;
                }
                else return;
            }
            else return;

            Hyperlink link = new Hyperlink(new Run(newText))
            {
                Name = Regex.Replace(newText, @"\s+", "")
            };
            HyperlinkMetadata.SetLinkType(link, linkType);
            HyperlinkMetadata.SetLinkKey(link, newText);
            link.TextDecorations = TextDecorations.Underline;

            range.Text = "";
            // Insert at the exact token location
            TextPointer insertPos = range.Start.GetInsertionPosition(LogicalDirection.Forward);
            Paragraph para = insertPos.Paragraph!;
            Inline? nextInline = insertPos.GetAdjacentElement(LogicalDirection.Forward) as Inline;


            if (nextInline != null)
                para.Inlines.InsertBefore(nextInline, link);
            else
                para.Inlines.Add(link);

            // Move caret after hyperlink
            rtb.CaretPosition = link.ElementEnd;

            Debug.WriteLine("Created hyperlink for " + link.Tag);

        }
        private static TextPointer? GetTextPointerAtOffset(TextPointer start, int offset)
        {
            TextPointer? current = start;
            int remaining = offset;

            while (current != null)
            {
                if (current.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string? textRun = current.GetTextInRun(LogicalDirection.Forward);
                    if (textRun != null)
                    {
                        if (remaining <= textRun.Length)
                            return current.GetPositionAtOffset(remaining);
                        remaining -= textRun.Length;
                    }
                }
                current = current.GetNextContextPosition(LogicalDirection.Forward);
            }

            return null;
        }

        private static FlowDocument CloneDocument(FlowDocument source)
        {
            if (source == null)
                return new FlowDocument();

            Dictionary<string, LinkType> keyRefPairs = new Dictionary<string, LinkType>();

            DefineLinkPairs(source, keyRefPairs);
            //DebugPrintXaml(source);
            TextRange range = new TextRange(
                source.ContentStart,
                source.ContentEnd);

            using var stream = new System.IO.MemoryStream();
            range.Save(stream, DataFormats.Xaml);

            stream.Position = 0;

            FlowDocument clone = new FlowDocument();
            TextRange cloneRange = new TextRange(
                clone.ContentStart,
                clone.ContentEnd);

            cloneRange.Load(stream, DataFormats.Xaml);

            RehydrateHyperlinkTags(clone, keyRefPairs);
            //DebugPrintXaml(clone);
            return clone;
        }
        public static void DebugPrintXaml(FlowDocument doc)
        {
            if (doc == null)
                return;

            var range = new TextRange(doc.ContentStart, doc.ContentEnd);

            using var stream = new MemoryStream();
            range.Save(stream, DataFormats.Xaml);

            stream.Position = 0;

            using var reader = new StreamReader(stream);
            string xaml = reader.ReadToEnd();

            Debug.WriteLine(xaml);
        }

        private static void DefineLinkPairs(FlowDocument doc, Dictionary<string, LinkType> dict)
        {
            foreach (Block block in doc.Blocks) DefineBlock(block, dict);
        }
        static void DefineBlock(Block block, Dictionary<string, LinkType> dict)
        {
            switch (block)
            {
                case Paragraph p:
                    foreach (Inline inline in p.Inlines)
                        DefineInline(inline, dict);
                    break;

                case Section s:
                    foreach (Block b in s.Blocks)
                        DefineBlock(b, dict);
                    break;

                case List l:
                    foreach (ListItem item in l.ListItems)
                        foreach (Block b in item.Blocks)
                            DefineBlock(b, dict);
                    break;
            }
        }
        static void DefineInline(Inline inline, Dictionary<string, LinkType> dict)
        {
            if (inline is Hyperlink link)
            {
                var type = HyperlinkMetadata.GetLinkType(link);
                var key = HyperlinkMetadata.GetLinkKey(link);

                if (key != null)
                {
                    dict[key] = type;
                }
            }
            else if (inline is Span span)
            {
                foreach (Inline child in span.Inlines)
                    DefineInline(child, dict);
            }
        }

        private static void RehydrateHyperlinkTags(FlowDocument doc, Dictionary<string, LinkType> dict)
        {
            foreach (Block block in doc.Blocks)
                RehydrateBlock(block, dict);
        }
        static void RehydrateBlock(Block block, Dictionary<string, LinkType> dict)
        {
            switch (block)
            {
                case Paragraph p:
                    foreach (Inline inline in p.Inlines)
                        RehydrateInline(inline, dict);
                    break;

                case Section s:
                    foreach (Block b in s.Blocks)
                        RehydrateBlock(b, dict);
                    break;

                case List l:
                    foreach (ListItem item in l.ListItems)
                        foreach (Block b in item.Blocks)
                            RehydrateBlock(b, dict);
                    break;
            }
        }

        static void RehydrateInline(Inline inline, Dictionary<string, LinkType> dict)
        {
            if (inline is Hyperlink link)
            {
                string key = new TextRange(link.ContentStart, link.ContentEnd).Text.Trim();

                if (dict.TryGetValue(key, out LinkType type))
                {
                    // Reattach metadata
                    HyperlinkMetadata.SetLinkKey(link, key);
                    HyperlinkMetadata.SetLinkType(link, type);

                    // Optional: restore visual style
                    link.TextDecorations = TextDecorations.Underline;
                    link.Foreground = Brushes.Gray;

                    Debug.WriteLine($"Rehydrated hyperlink: {key} ({type})");
                }
            }
            else if (inline is Span span)
            {
                foreach (Inline child in span.Inlines)
                    RehydrateInline(child, dict);
            }
        }

        /// <summary>
        /// Clicking the hyperlink in the FlowDocument activates a window for quick info.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RichTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not RichTextBox rtb) return;

            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                return;

            DependencyObject current = e.OriginalSource as DependencyObject;

            while (current != null && current is not Hyperlink)
                current = LogicalTreeHelper.GetParent(current);

            Hyperlink? link = current as Hyperlink;


            if (link == null)
                return;

            e.Handled = true; // prevent caret from moving

            string key = HyperlinkMetadata.GetLinkKey(link);
            LinkType type = HyperlinkMetadata.GetLinkType(link);
            Debug.WriteLine($"Clicked on link... {key}.");

            if (key != string.Empty)
            {
                object taggedObj = HyperlinkHelper.GetObject(type, key);
                ShowPopup(taggedObj);

            }
            else
            {
                Debug.WriteLine($"Tag is not a linkRef!");

            }
        }
        private static void ShowPopup(object context)
        {
            if (context == null) return;

            
            object viewModel = context switch
            {
                Spell s => new SpellViewModel(s),
                Item i => new ItemViewModel(i),
                Monster m => new MonsterViewModel(m),
                NPC n => new NPCViewModel(n),
                Location l => new LocationViewModel(l),
                _ => throw new ArgumentException($"Unsupported type: {context.GetType()}")
            };

            var window = new PopupContextWindow
            {
                DataContext = new PopupContextWindowViewModel(viewModel)
            };
            window.Show();
        }


    }
}
