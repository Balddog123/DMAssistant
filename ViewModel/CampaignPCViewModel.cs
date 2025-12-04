using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Helpers;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace DMAssistant.ViewModel
{
    public class CampaignPCViewModel : ObservableObject
    {
        private ObservableCollection<PlayerCharacter> _playerCharacters = new();
        public ObservableCollection<PlayerCharacter> PlayerCharacters
        {
            get => _playerCharacters;
            set
            {
                SetProperty(ref _playerCharacters, value);
            }
        }

        private Item _selectedItem;
        public Item ExpandedItem
        {
            get => _selectedItem;
            set
            {
                if(_selectedItem != null) _selectedItem.ExpandedVisibility = Visibility.Collapsed;

                if (SetProperty(ref _selectedItem, value))
                {
                    ExpandedItemViewModel = new ItemViewModel(_selectedItem);
                    if (_selectedItem != null) _selectedItem.ExpandedVisibility = Visibility.Visible;
                }
            }
        }
        private ItemViewModel _selectedItemViewModel;
        public ItemViewModel ExpandedItemViewModel
        {
            get => _selectedItemViewModel;
            set => SetProperty(ref _selectedItemViewModel, value);
        }

        private Dictionary<Item, List<PlayerCharacter>> itemToPlayers = new Dictionary<Item, List<PlayerCharacter>>();
        public ObservableCollection<ItemViewModel> MajorItems { get; } = new();
        public ObservableCollection<ItemViewModel> MinorItems { get; } = new();
        public ICollectionView MajorItemsView { get; }
        public ICollectionView MinorItemsView { get; }
        private int _partyLevel
        {
            get
            {
                int level = 0;
                foreach (PlayerCharacter character in PlayerCharacters)
                {
                    level += character.level;
                }
                level /= PlayerCharacters.Count;
                return level;
            }
        }
        public int GetMaxItemsByRank(Item.ItemRank rank, Item.ItemType type)
        {
            int partyLevel = _partyLevel;
            if(type == Item.ItemType.Minor)
            {
                if(partyLevel <= 4)
                {
                    return rank switch
                    {
                        Item.ItemRank.Common => 6,
                        Item.ItemRank.Uncommon => 2,
                        Item.ItemRank.Rare => 1,
                        Item.ItemRank.VeryRare => 0,
                        Item.ItemRank.Legendary => 0,
                        _ => 0
                    };
                }
                else if(partyLevel <= 10)
                {
                    return rank switch
                    {
                        Item.ItemRank.Common => 16,
                        Item.ItemRank.Uncommon => 14,
                        Item.ItemRank.Rare => 5,
                        Item.ItemRank.VeryRare => 1,
                        Item.ItemRank.Legendary => 0,
                        _ => 0
                    };
                }
                else if (partyLevel <= 16)
                {
                    return rank switch
                    {
                        Item.ItemRank.Common => 19,
                        Item.ItemRank.Uncommon => 20,
                        Item.ItemRank.Rare => 15,
                        Item.ItemRank.VeryRare => 6,
                        Item.ItemRank.Legendary => 1,
                        _ => 0
                    };
                }
                else
                {
                    return rank switch
                    {
                        Item.ItemRank.Common => 19,
                        Item.ItemRank.Uncommon => 20,
                        Item.ItemRank.Rare => 19,
                        Item.ItemRank.VeryRare => 15,
                        Item.ItemRank.Legendary => 7,
                        _ => 0
                    };
                }
            }
            else
            {
                if (partyLevel <= 4)
                {
                    return rank switch
                    {
                        Item.ItemRank.Common => 99,
                        Item.ItemRank.Uncommon => 2,
                        Item.ItemRank.Rare => 0,
                        Item.ItemRank.VeryRare => 0,
                        Item.ItemRank.Legendary => 0,
                        _ => 0
                    };
                }
                else if (partyLevel <= 10)
                {
                    return rank switch
                    {
                        Item.ItemRank.Common => 99,
                        Item.ItemRank.Uncommon => 7,
                        Item.ItemRank.Rare => 1,
                        Item.ItemRank.VeryRare => 0,
                        Item.ItemRank.Legendary => 0,
                        _ => 0
                    };
                }
                else if (partyLevel <= 16)
                {
                    return rank switch
                    {
                        Item.ItemRank.Common => 99,
                        Item.ItemRank.Uncommon => 8,
                        Item.ItemRank.Rare => 3,
                        Item.ItemRank.VeryRare => 2,
                        Item.ItemRank.Legendary => 1,
                        _ => 0
                    };
                }
                else
                {
                    return rank switch
                    {
                        Item.ItemRank.Common => 99,
                        Item.ItemRank.Uncommon => 8,
                        Item.ItemRank.Rare => 4,
                        Item.ItemRank.VeryRare => 4,
                        Item.ItemRank.Legendary => 4,
                        _ => 0
                    };
                }
            }
        }

        public ICommand AddItemCommand { get; }
        public ICommand AddNewPlayerCommand { get; }
        public ICommand DeleteItemCommand => new RelayCommand<RemoveItemRequest>(req =>
        {
            Debug.WriteLine($"Attempting to delete {req.Item?.Name} from {req.Player?.Name}...");
            if (req.Player != null && req.Item != null)
            {
                req.Player.Items.Remove(req.Item);
                try
                {
                    itemToPlayers[req.Item].Remove(req.Player);
                }catch(Exception e)
                {

                }
            }
        });
        public ICommand DeletePlayerCommand => new RelayCommand<PlayerCharacter>(player =>
        {
            if (MessageBox.Show($"Delete {player.Name}?",
                                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                PlayerCharacters.Remove(player);
            }
        });

        public CampaignPCViewModel()
        {
            PlayerCharacters = App.CampaignStore.CurrentCampaign.PCs;
            AddItemCommand = new RelayCommand<PlayerCharacter>(player =>
            {
                var available = App.CampaignStore.CurrentCampaign.Items.ToList();

                if (!available.Any())
                {
                    MessageBox.Show("No items available!");
                    return;
                }

                var window = new SelectItemWindow(available);
                if (window.ShowDialog() == true && window.SelectedItem != null)
                {
                    var item = window.SelectedItem;
                    player.Items.Add(item);

                    CreateItemViewModel(player, item);
                }
            });
            AddNewPlayerCommand = new RelayCommand(() =>
            {
                PlayerCharacters.Add(new PlayerCharacter());
            });            

            MajorItemsView = CollectionViewSource.GetDefaultView(MajorItems);
            MajorItemsView.SortDescriptions.Add(new SortDescription(nameof(Item.Rank), ListSortDirection.Ascending));
            MajorItemsView.GroupDescriptions.Add(new PropertyGroupDescription("Item.Rank"));

            MinorItemsView = CollectionViewSource.GetDefaultView(MinorItems);
            MinorItemsView.SortDescriptions.Add(new SortDescription(nameof(Item.Rank), ListSortDirection.Ascending));
            MinorItemsView.GroupDescriptions.Add(new PropertyGroupDescription("Item.Rank"));

            foreach (PlayerCharacter player in PlayerCharacters)
            {
                player.PropertyChanged += PlayerCharacter_PropertyChanged;

                foreach (Item item in player.Items)
                {
                    CreateItemViewModel(player, item);
                }
            }

            PlayerCharacters.CollectionChanged += PlayerCharacters_CollectionChanged;

            OnPropertyChanged(nameof(MajorItems));
            OnPropertyChanged(nameof(MinorItems));

        }

        private void PlayerCharacters_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (PlayerCharacter newPc in e.NewItems)
                    newPc.PropertyChanged += PlayerCharacter_PropertyChanged;

            if (e.OldItems != null)
                foreach (PlayerCharacter oldPc in e.OldItems)
                    oldPc.PropertyChanged -= PlayerCharacter_PropertyChanged;
        }

        private void PlayerCharacter_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlayerCharacter.Level))
            {
                // Refresh the grouped views so headers update
                
                MajorItemsView.Refresh();
                MinorItemsView.Refresh();
            }
        }

        private void CreateItemViewModel(PlayerCharacter player, Item item)
        {
            if (!itemToPlayers.TryGetValue(item, out var list) || list == null) itemToPlayers[item] = new List<PlayerCharacter>();
            itemToPlayers[item].Add(player);

            Debug.WriteLine("Creating item view for item list...");
            var vm = new ItemViewModel(item, player);
            if (item.Type == Item.ItemType.Major)
                MajorItems.Add(vm);
            else
                MinorItems.Add(vm);

            Debug.WriteLine("Setting property change events for item " + item.Name);
            item.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Item.Type))
                {
                    Debug.WriteLine("Change type, updating major and minor items now...");
                    if (vm.Item.Type == Item.ItemType.Major)
                    {
                        MinorItems.Remove(vm);
                        if (!MajorItems.Contains(vm)) MajorItems.Add(vm);
                    }
                    if (vm.Item.Type == Item.ItemType.Minor)
                    {
                        MajorItems.Remove(vm);
                        if (!MinorItems.Contains(vm)) MinorItems.Add(vm);
                    }
                }
            };
        }

        private void UpdateItemExpansion()
        {
            foreach (var pc in PlayerCharacters)
            {
                foreach (var item in pc.Items)
                {
                    item.ExpandedVisibility = (item == ExpandedItem) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                }
            }
        }
    }
}
