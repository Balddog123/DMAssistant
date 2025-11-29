using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Helpers
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public static class ScrollViewerUtilities
    {
        public static readonly DependencyProperty BubbleMouseWheelProperty =
            DependencyProperty.RegisterAttached(
                "BubbleMouseWheel",
                typeof(bool),
                typeof(ScrollViewerUtilities),
                new UIPropertyMetadata(false, OnBubbleMouseWheelChanged));

        public static bool GetBubbleMouseWheel(DependencyObject obj)
            => (bool)obj.GetValue(BubbleMouseWheelProperty);

        public static void SetBubbleMouseWheel(DependencyObject obj, bool value)
            => obj.SetValue(BubbleMouseWheelProperty, value);

        private static void OnBubbleMouseWheelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                    element.PreviewMouseWheel += OnPreviewMouseWheel;
                else
                    element.PreviewMouseWheel -= OnPreviewMouseWheel;
            }
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not UIElement element)
                return;

            // Always forward mouse wheel to nearest ScrollViewer parent
            var scrollViewer = FindParentScrollViewer(element);
            if (scrollViewer == null)
                return;

            if (e.Delta > 0)
                scrollViewer.LineUp();
            else
                scrollViewer.LineDown();

            e.Handled = true;
        }

        private static ScrollViewer? FindParentScrollViewer(DependencyObject child)
        {
            var parent = VisualTreeHelper.GetParent(child);
            while (parent != null)
            {
                if (parent is ScrollViewer sv)
                    return sv;

                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }
    }

}
