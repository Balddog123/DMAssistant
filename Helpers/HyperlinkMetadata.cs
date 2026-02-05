using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace DMAssistant.Helpers
{
    public static class HyperlinkMetadata
    {
        public static readonly DependencyProperty LinkTypeProperty =
         DependencyProperty.RegisterAttached(
             "LinkType",
             typeof(LinkType),          // enum → serializable
             typeof(HyperlinkMetadata),
             new FrameworkPropertyMetadata(
                 default(LinkType),
                 FrameworkPropertyMetadataOptions.Inherits));

        public static readonly DependencyProperty LinkKeyProperty =
            DependencyProperty.RegisterAttached(
                "LinkKey",
                typeof(string),            // string → serializable
                typeof(HyperlinkMetadata),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.Inherits));

        public static void SetLinkType(DependencyObject obj, LinkType value)
            => obj.SetValue(LinkTypeProperty, value);

        public static LinkType GetLinkType(DependencyObject obj)
            => (LinkType)obj.GetValue(LinkTypeProperty);

        public static void SetLinkKey(DependencyObject obj, string value)
            => obj.SetValue(LinkKeyProperty, value);

        public static string GetLinkKey(DependencyObject obj)
            => (string)obj.GetValue(LinkKeyProperty);
    }


}
