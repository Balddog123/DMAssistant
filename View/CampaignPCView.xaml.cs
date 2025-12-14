using DMAssistant.Model;
using DMAssistant.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace DMAssistant.View
{
    public partial class CampaignPCView : UserControl
    {
        public CampaignPCView()
        {
            InitializeComponent();

        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Get the element that was clicked
            var clickedElement = e.OriginalSource as DependencyObject;
            if (clickedElement == null) return;

            CampaignPCViewModel vm = null;
            if (DataContext is CampaignPCViewModel)
            {
                vm = DataContext as CampaignPCViewModel;
            }

            // Check if the click was inside a ListBoxItem
            var listBoxItem = clickedElement.FindAncestor<ListBoxItem>();
            if (listBoxItem != null)
            {
                // Clicked inside an item, do nothing
                Item clickedItem = listBoxItem.DataContext as Item;
                if (vm != null && vm.SelectedItem == clickedItem)
                {
                    vm.SelectedItem = null;
                    e.Handled = true;
                    Debug.WriteLine("Expanded clicked by itself!");
                }
                return;
            }

            // Check if the click was inside a ListBox itself (but not on an item)
            var listBox = clickedElement.FindAncestor<ListBox>();
            if (listBox != null)
            {
                // Clicked inside the ListBox but not on an item
                // Optional: you could collapse the currently expanded item here too
                return;
            }

            // Clicked outside the items; collapse expanded item
            if (vm != null)
            {
                vm.SelectedItem = null;
            }
        }
    }

    public static class VisualTreeHelpers
    {
        /// <summary>
        /// Walks up the visual tree to find the first ancestor of type T
        /// </summary>
        public static T? FindAncestor<T>(this DependencyObject? child) where T : DependencyObject
        {
            while (child != null && !(child is T))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as T;
        }
    }
}
