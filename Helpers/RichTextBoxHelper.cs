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

        private static void OnBoundDocumentChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not RichTextBox rtb || GetIsUpdating(rtb))
                return;

            // Detach old handler
            rtb.TextChanged -= RichTextBox_TextChanged;

            if (e.NewValue is FlowDocument newDoc)
            {
                // Assign document
                rtb.Document = CloneDocument(newDoc);

                // Attach handler
                rtb.TextChanged += RichTextBox_TextChanged;
                rtb.PreviewMouseLeftButtonDown += RichTextBox_PreviewMouseLeftButtonDown;

            }
            else
            {
                rtb.Document = new FlowDocument();
            }

            Debug.WriteLine(e.NewValue);
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
                SetBoundDocument(rtb, currentDoc);
            }
            finally
            {
                SetIsUpdating(rtb, false);
            }
            Debug.WriteLine("Changed");
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

            if (type == 's')
            {
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
                Tag = objectForLink
            };

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

            return clone;
        }
        private static void RichTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not RichTextBox rtb) return;

            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                return;

            // Walk up the visual/logical tree to find the Hyperlink
            Hyperlink? link = null;

            if (e.OriginalSource is Run run)
                link = run.Parent as Hyperlink;
            else if (e.OriginalSource is Hyperlink directLink)
                link = directLink;

            if (link == null)
                return;

            e.Handled = true; // prevent caret from moving

            // Now read the tag
            var tag = link.Tag;
            if (tag != null)
            {
                ShowPopup(tag);
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
