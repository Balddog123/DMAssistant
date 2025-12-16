using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace DMAssistant.ViewModel
{
    public class CampaignLoreViewModel : ObservableObject
    {

        private ObservableCollection<Lore> _lore;
        public ObservableCollection<Lore> Lore
        {
            get => _lore;
            set
            {
                if (SetProperty(ref _lore, value))
                {
                    // Update the underlying Campaign object
                    App.CampaignStore.CurrentCampaign.Lore = value;

                }
            }
        }

        public ICollectionView LoreView { get; }

        private Lore _selectedLore;
         public Lore SelectedLore
        {
            get => _selectedLore;
            set
            {
                SetProperty(ref _selectedLore, value);
            }
        }

        public IRelayCommand AddLore { get; }
        public IRelayCommand DeleteLore { get; }

        public CampaignLoreViewModel()
        {
            _lore = App.CampaignStore.CurrentCampaign.Lore;
            LoreView = CollectionViewSource.GetDefaultView(_lore);
            LoreView.Filter = FilterLore;
            ApplyFilters();

            AddLore = new RelayCommand(() => Lore.Add(new Lore()));
            DeleteLore = new RelayCommand<Lore>(lore =>
            {
                if (MessageBox.Show($"Delete {lore.Name}?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
                App.CampaignStore.CurrentCampaign.Lore.Remove(lore);
                Lore.Remove(lore);
                if (SelectedLore == lore) SelectedLore = Lore.FirstOrDefault();
            });
        }

        private bool FilterLore(object obj)
        {
            if (obj is not Lore m) return false;

            /*
            if (!string.IsNullOrWhiteSpace(Search) &&
                !m.Name.Contains(Search, StringComparison.OrdinalIgnoreCase))
                return false;
            */

            return true;
        }

        private void ApplyFilters()
        {
            LoreView.Refresh();
        }
    }
}
