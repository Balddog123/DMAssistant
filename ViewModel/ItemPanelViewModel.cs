using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;

namespace DMAssistant.ViewModel
{
    public partial class ItemPanelViewModel : ObservableObject
    {
        public Session _session { get; private set; }

        [ObservableProperty] public ObservableCollection<ItemViewModel> itemList;


        private ItemViewModel _selectedItem;
        public ItemViewModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        //FILTERING
        public ICollectionView ItemsView { get; }
        private string _search = "";
        public string Search
        {
            get => _search;
            set
            {
                if (SetProperty(ref _search, value)) ApplyFilters();
            }
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

            if(session == null) App.CampaignStore.CurrentCampaign.Items.CollectionChanged += OnCampaignItemsChanged;

            if (ItemList.Any()) SelectedItem = ItemList[0];

            AddItemCommand = new RelayCommand(() => AddItem());
            AddExistingItemCommand = new RelayCommand(AddExistingItem);
            DeleteItemCommand = new RelayCommand<ItemViewModel>(DeleteItem);
            DuplicateItem = new RelayCommand<ItemViewModel>(itemVM => { AddItem(itemVM.Item); });

            // Watch for per-item changes
            foreach (ItemViewModel vm in ItemList) HookItemEvents(vm);

            App.CampaignStore.ItemDeleted += OnItemDeleted;

            ItemsView = CollectionViewSource.GetDefaultView(ItemList);
            ItemsView.SortDescriptions.Add(new SortDescription(nameof(ItemViewModel.IsNew), ListSortDirection.Descending));
            ItemsView.SortDescriptions.Add(new SortDescription(nameof(ItemViewModel.Name), ListSortDirection.Ascending));
            ItemsView.Filter = FilterItems;
            ApplyFilters();
        }

        private bool FilterItems(object obj)
        {
            if (obj is not ItemViewModel m) return false;

            if (!string.IsNullOrWhiteSpace(Search) &&
                !m.Name.Contains(Search, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
        private void ApplyFilters()
        {
            ItemsView.Refresh();
        }

        private void OnCampaignItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Debug.WriteLine("CampaignItemsChanged..");
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Item item in e.NewItems!)
                    {
                        Debug.WriteLine("Adding new Item to list..");

                        var vm = CreateItemViewModel(item);
                        ItemList.Insert(0, vm);
                    }
                    
                    break;

                //case NotifyCollectionChangedAction.Remove:
                //    Debug.WriteLine("Removed-item callback from CampaignStore...");
                //    foreach (Item item in e.OldItems!)
                //    {
                //        Debug.WriteLine($"Item to remove: {item.ID}");

                //        foreach (ItemViewModel model in ItemList)
                //        {
                //            if (model.Item.ID == item.ID)
                //            {
                //                Debug.WriteLine($"ID matches. Removing from panel list.");

                //                ItemList.Remove(model);
                //                break;
                //            }
                //            else
                //            {
                //                Debug.WriteLine($"ID doesn't match: {model.Item.ID}");

                //            }
                //        }
                //    }
                //    break;

                case NotifyCollectionChangedAction.Reset:
                    ItemList.Clear();
                    break;
            }
        }

        public void Dispose()
        {
            if(_session == null) App.CampaignStore.CurrentCampaign.Items.CollectionChanged -= OnCampaignItemsChanged;
        }

        private ItemViewModel CreateItemViewModel(Item item)
        {
            var vm = new ItemViewModel(item);
            // listen to whenever Name/Rank/etc. changes
            HookItemEvents(vm);
            Debug.WriteLine(vm.IsNew);
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
                    ApplyFilters();
                }
            };
        }

        private void AddItem(Item itemToCopy = null)
        {
            var item = itemToCopy != null ? new Item(itemToCopy) : new Item() { IsNew = true };
            int index = App.CampaignStore.CurrentCampaign.Items.IndexOf(itemToCopy) + 1;

            App.CampaignStore.CurrentCampaign.Items.Insert(index, item);
            App.CampaignStore.ItemIndex[item.ID] = item;

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

            if (_session != null)
            {
                _session.ItemIDs.Add(item.ID);

                var vm = CreateItemViewModel(item);
                ItemList.Insert(0, vm);
                SelectedItem = vm;
            }
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
                ItemList.Insert(0, vm);
                SelectedItem = vm;
            }
        }

        private void DeleteItem(ItemViewModel itemVM)
        {
            if (MessageBox.Show($"Delete {itemVM.Name}?",
                "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            // Delete from campaign
            App.CampaignStore.DeleteItem(itemVM.Item, this);

            // Delete from UI
            ItemList.Remove(itemVM);

            // Select something new
            if (SelectedItem == itemVM)
                SelectedItem = ItemList.FirstOrDefault();
        }

        private void OnItemDeleted(ItemPanelViewModel panel, Item item)
        {
            Debug.WriteLine($"---Calling OnItemDeleted event at {this}---");

            ItemViewModel vm = ItemList.FirstOrDefault(x => x.Item.ID == item.ID);
            if(vm != null)
            {
                Debug.WriteLine($"Found vm: {vm}, {vm.Item}, {vm.Item.Name}...");
                ItemList.Remove(vm);

                if (SelectedItem == vm) SelectedItem = ItemList.FirstOrDefault();
            }
            
        }
    }


}
