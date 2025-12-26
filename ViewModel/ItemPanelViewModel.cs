using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace DMAssistant.ViewModel
{
    public partial class ItemPanelViewModel : ObservableObject
    {
        private readonly Session _session;

        [ObservableProperty] public ObservableCollection<ItemViewModel> itemList;


        private ItemViewModel _selectedItem;
        public ItemViewModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public IRelayCommand AddItemCommand { get; }
        public IRelayCommand AddExistingItemCommand { get; }
        public IRelayCommand<ItemViewModel> DeleteItemCommand { get; }
        public RelayCommand<ItemViewModel> DuplicateItem { get; }

        public ItemPanelViewModel(ObservableCollection<string> ids, Session session)
        {
            _session = session;

            // Convert item IDs → viewmodels
            ItemList = new ObservableCollection<ItemViewModel>(ids.Select(id => CreateItemViewModel(App.CampaignStore.ItemIndex[id])));

            if (ItemList.Any()) SelectedItem = ItemList[0];

            AddItemCommand = new RelayCommand(() => AddItem());
            AddExistingItemCommand = new RelayCommand(AddExistingItem);
            DeleteItemCommand = new RelayCommand<ItemViewModel>(DeleteItem);
            DuplicateItem = new RelayCommand<ItemViewModel>(itemVM => { AddItem(itemVM.Item); });

            // Watch for per-item changes
            foreach (ItemViewModel vm in ItemList)
                HookItemEvents(vm);
        }

        private ItemViewModel CreateItemViewModel(Item item)
        {
            var vm = new ItemViewModel(item);
            
            // listen to whenever Name/Rank/etc. changes
            HookItemEvents(vm);

            return vm;
        }

        private void HookItemEvents(ItemViewModel vm)
        {
            vm.PropertyChanged += (_, args) =>
            {
                // Example: react to name changes
                if (args.PropertyName == nameof(ItemViewModel.Name))
                {
                    // Raise panel-level update (e.g., refresh list)
                    OnPropertyChanged(nameof(ItemList));
                }
            };
        }

        private void AddItem(Item itemToCopy = null)
        {
            var item = itemToCopy != null ? new Item(itemToCopy) : new Item();
            int index = App.CampaignStore.CurrentCampaign.Items.IndexOf(itemToCopy) + 1;

            App.CampaignStore.CurrentCampaign.Items.Insert(index, item);
            App.CampaignStore.ItemIndex[item.ID] = item;

            if(_session != null) _session.ItemIDs.Add(item.ID);

            if (itemToCopy == null)
            {
                item.Name = Item.GetRandomName();
                item.Origin = new FlowDocument();
                item.Origin.Blocks.Add(new Paragraph(new Run(Item.GetRandomItemOrigin())));
                item.Function = new FlowDocument();
                item.Function.Blocks.Add(new Paragraph(new Run(Item.GetRandomFunction())));
                item.Appearance = new FlowDocument();
                item.Appearance.Blocks.Add(new Paragraph(new Run(Item.GetRandomAppearance())));
            }

            var vm = CreateItemViewModel(item);
            InsertSorted(vm);
            SelectedItem = vm;
        }
        private void InsertSorted(ItemViewModel vm)
        {
            if (ItemList.Count == 0)
            {
                ItemList.Add(vm);
                return;
            }

            // Find the index where the item should go
            int index = 0;
            while (index < ItemList.Count && string.Compare(ItemList[index].Name, vm.Name, StringComparison.OrdinalIgnoreCase) < 0)
            {
                index++;
            }

            ItemList.Insert(index, vm);
        }


        private void AddExistingItem()
        {
            if (_session == null) return;

            var available = App.CampaignStore.CurrentCampaign.Items;

            if (!available.Any())
            {
                MessageBox.Show("No Items available!");
                return;
            }

            var window = new SelectItemWindow(available.ToList());
            if (window.ShowDialog() == true && window.SelectedItem != null)
            {
                var item = window.SelectedItem;
                _session.ItemIDs.Add(item.ID);

                var vm = CreateItemViewModel(item);
                InsertSorted(vm);
                SelectedItem = vm;
            }
        }

        private void DeleteItem(ItemViewModel itemVM)
        {
            if (MessageBox.Show($"Delete {itemVM.Name}?",
                "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            // Delete from campaign
            App.CampaignStore.DeleteItem(itemVM.Item, _session);

            // Delete from UI
            ItemList.Remove(itemVM);

            // Select something new
            if (SelectedItem == itemVM)
                SelectedItem = ItemList.FirstOrDefault();
        }
    }


}
