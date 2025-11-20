using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Helpers;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
        public ICommand AddItemCommand { get; }
        public ICommand AddNewPlayerCommand { get; }
        public ICommand DeleteItemCommand => new RelayCommand<RemoveItemRequest>(req =>
        {
            Debug.WriteLine($"Attempting to delete {req.Item?.Name} from {req.Player?.Name}...");
            if (req.Player != null && req.Item != null)
            {
                req.Player.Items.Remove(req.Item);
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
                    Debug.WriteLine(item);
                    player.Items.Add(item);
                }
            });
            AddNewPlayerCommand = new RelayCommand(() =>
            {
                PlayerCharacters.Add(new PlayerCharacter());
            });
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
