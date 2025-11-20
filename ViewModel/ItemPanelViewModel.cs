using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DMAssistant.ViewModel
{
    public class ItemPanelViewModel : ObservableObject
    {
        private readonly ObservableCollection<string> _sessionItemIDs;
        public Session _session;
        public ObservableCollection<Item> ItemList { get; }

        private Item _selectedItem;
        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    SelectedItemViewModel = new ItemViewModel(_selectedItem);
                }
            }
        }

        private ItemViewModel _selectedItemViewModel;
        public ItemViewModel SelectedItemViewModel
        {
            get => _selectedItemViewModel;
            set => SetProperty(ref _selectedItemViewModel, value);
        }

        public IRelayCommand AddItemCommand { get; }
        public IRelayCommand AddExistingItemCommand { get; }
        public IRelayCommand DeleteItemCommand => new RelayCommand<Item>(itemToRemove =>
        {
            if (MessageBox.Show($"Delete {itemToRemove.Name}?",
                                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                RemoveItem(itemToRemove);
            }
        });

        public ItemPanelViewModel(ObservableCollection<string> itemIDs, Session session)
        {
            _sessionItemIDs = itemIDs;
            ItemList = new ObservableCollection<Item>();

            // Hydrate real Monster objects
            foreach (string id in itemIDs)
            {
                if (App.CampaignStore.ItemIndex.TryGetValue(id, out var item))
                    ItemList.Add(item);
            }

            if (ItemList.Any()) SelectedItem = ItemList[0];

            AddItemCommand = new RelayCommand(AddItem);
            AddExistingItemCommand = new RelayCommand(AddExistingItem);

            _session = session;
            App.CampaignStore.ItemDeleted += OnItemDeleted;
        }

        private void RemoveItem(Item? itemToRemove)
        {
            if (itemToRemove != null)
            {
                ItemList.Remove(itemToRemove);
            }
        }

        private void OnItemDeleted(Item item)
        {
            if (ItemList.Contains(item)) ItemList.Remove(item);

            if (SelectedItem == item) SelectedItem = ItemList.FirstOrDefault();
        }

        private void AddItem()
        {
            var item = new Item("New Item", Item.ItemRank.Common, "", "", "", "");
            ItemList.Add(item);
            SelectedItem = item;
            // Item belongs to global campaign list
            App.CampaignStore.CurrentCampaign.Items.Add(item);
            App.CampaignStore.ItemIndex[item.ID] = item;
            // Add ID to session
            _sessionItemIDs.Add(item.ID);
        }

        private void AddExistingItem()
        {
            // Open a simple selection dialog
            var availableItems = App.CampaignStore.CurrentCampaign.Items.ToList();

            if (!availableItems.Any())
            {
                MessageBox.Show("No Items available!");
                return;
            }

            var window = new SelectItemWindow(availableItems);
            var result = window.ShowDialog();

            if (result == true && window.SelectedItem != null)
            {
                _sessionItemIDs.Add(window.SelectedItem.ID);
                ItemList.Add(window.SelectedItem);
                SelectedItem = window.SelectedItem;
            }
        }
    }

}
