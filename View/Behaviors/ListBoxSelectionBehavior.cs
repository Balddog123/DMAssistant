using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace DMAssistant.View.Behaviors
{
    public static class ListBoxSelectionBehavior
    {
        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(ListBoxSelectionBehavior), new PropertyMetadata(false));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(IList),
                typeof(ListBoxSelectionBehavior),
                new PropertyMetadata(null, OnSelectedItemsChanged));

        public static IList GetSelectedItems(DependencyObject obj) => (IList)obj.GetValue(SelectedItemsProperty);
        public static void SetSelectedItems(DependencyObject obj, IList value) => obj.SetValue(SelectedItemsProperty, value);

        private static bool GetIsUpdating(DependencyObject obj) => (bool)obj.GetValue(IsUpdatingProperty);
        private static void SetIsUpdating(DependencyObject obj, bool value) => obj.SetValue(IsUpdatingProperty, value);

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBox listBox) return;

            // detach old handlers
            listBox.SelectionChanged -= ListBox_SelectionChanged;

            if (e.OldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= BoundCollection_CollectionChanged;

            // attach new handlers
            listBox.SelectionChanged += ListBox_SelectionChanged;

            if (e.NewValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += BoundCollection_CollectionChanged;

            // Initialize ListBox.SelectedItems to match bound collection
            if (!GetIsUpdating(listBox) && e.NewValue is IList newList)
            {
                try
                {
                    SetIsUpdating(listBox, true);
                    listBox.SelectedItems.Clear();
                    foreach (var item in newList)
                    {
                        if (listBox.Items.Contains(item))
                            listBox.SelectedItems.Add(item);
                    }
                }
                finally
                {
                    SetIsUpdating(listBox, false);
                }
            }
        }

        private static void ListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (sender is not ListBox listBox) return;
            var bound = GetSelectedItems(listBox);
            if (bound == null) return;
            if (GetIsUpdating(listBox)) return;

            try
            {
                SetIsUpdating(listBox, true);

                // Add newly selected items to bound list
                foreach (var added in e.AddedItems)
                {
                    if (!bound.Contains(added))
                        bound.Add(added);
                }

                // Remove unselected items from bound list
                foreach (var removed in e.RemovedItems)
                {
                    if (bound.Contains(removed))
                        bound.Remove(removed);
                }
            }
            finally
            {
                SetIsUpdating(listBox, false);
            }
        }

        private static void BoundCollection_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // sender is the bound IList which implements INotifyCollectionChanged
            // We need to find the ListBox that has this collection attached.
            // Looping through Application.Current.Windows to find matching one is expensive;
            // instead, we will attach the collection-changed handler only when property is set (done in OnSelectedItemsChanged),
            // and we can rely on the fact that the ListBox's SelectedItems should be updated by OnSelectedItemsChanged initial sync
            // and by this event only when it was changed programmatically (outside selection).
            //
            // Implementation detail: we cannot easily get the associated ListBox from the collection here.
            // But we already attached the handler from OnSelectedItemsChanged with the correct collection instance.
            // To update the ListBox, we will search visual tree for ListBoxes that reference this collection.
            // For simplicity and to keep the behavior dependable, we will update any ListBox found that has the same collection reference.

            if (sender == null) return;

            var collection = (IList)sender;

            // Find all ListBoxes in the application that have this collection attached and update them.
            foreach (Window w in System.Windows.Application.Current.Windows)
            {
                UpdateListBoxesInVisualTree(w, collection);
            }
        }

        private static void UpdateListBoxesInVisualTree(DependencyObject parent, IList collection)
        {
            var childCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

                if (child is ListBox lb)
                {
                    var attached = GetSelectedItems(lb);
                    if (ReferenceEquals(attached, collection))
                    {
                        try
                        {
                            SetIsUpdating(lb, true);
                            lb.SelectedItems.Clear();
                            foreach (var item in collection)
                            {
                                if (lb.Items.Contains(item))
                                    lb.SelectedItems.Add(item);
                            }
                        }
                        finally
                        {
                            SetIsUpdating(lb, false);
                        }
                    }
                }

                UpdateListBoxesInVisualTree(child, collection);
            }
        }
    }
}
