using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Helpers;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DMAssistant.ViewModel
{
    public partial class SpellsPanelViewModel : ObservableObject
    {
        [ObservableProperty] public ObservableCollection<SpellViewModel> spellList = new ObservableCollection<SpellViewModel>();
        public ICollectionView SpellsView { get; }

        [ObservableProperty] public SpellViewModel selectedSpell;

        private string _search = "";
        public string Search
        {
            get => _search;
            set
            {
                if (SetProperty(ref _search, value)) ApplyFilters();
            }
        }

        public IRelayCommand AddSpellCommand { get; }
        public IRelayCommand DeleteSpellCommand { get; }
        public SpellsPanelViewModel()
        {
            SpellList = new ObservableCollection<SpellViewModel>(
                App.CampaignStore.CurrentCampaign.Spells.OrderBy(s => s.Name).Select(spell => CreateSpellViewModel(spell, false))
            );

            AddSpellCommand = new RelayCommand(()=> SpellList.Insert(0, CreateSpellViewModel(CreateDefaultSpell(), true)));
            DeleteSpellCommand = new RelayCommand<SpellViewModel>(deleteSpell =>
            {
                if (MessageBox.Show($"Delete {deleteSpell.Spell.Name}?",
                                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SpellList.Remove(deleteSpell);
                }
                
            });

            SpellsView = CollectionViewSource.GetDefaultView(SpellList);
            SpellsView.SortDescriptions.Add(new SortDescription(nameof(SpellViewModel.IsNew), ListSortDirection.Descending));
            SpellsView.SortDescriptions.Add(new SortDescription(nameof(SpellViewModel.Name), ListSortDirection.Ascending));
            SpellsView.Filter = FilterSpell;
            ApplyFilters();
        }

        private bool FilterSpell(object obj)
        {
            if (obj is not SpellViewModel m) return false;

            if (!string.IsNullOrWhiteSpace(Search) &&
                !m.Name.Contains(Search, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
        private void ApplyFilters()
        {
            SpellsView.Refresh();
        }
        private SpellViewModel CreateSpellViewModel(Spell spell, bool isNew)
        {
            var vm = new SpellViewModel(spell);
            vm.IsNew = isNew;
            // listen to whenever Name/Rank/etc. changes
            HookSpellEvents(vm);

            Debug.WriteLine(vm.IsNew);
            return vm;
        }

        private void HookSpellEvents(SpellViewModel vm)
        {
            vm.PropertyChanged += (_, args) =>
            {
                Debug.WriteLine("Something was changed on spell...");
                // Example: react to name changes
                if (args.PropertyName == nameof(SpellViewModel.Name))
                {
                    
                    // Raise panel-level update (e.g., refresh list)
                    OnPropertyChanged(nameof(SpellList));
                    SpellsView.Refresh();
                }
            };
        }

        private Spell CreateDefaultSpell()
        {
            // You can customize this to match your default Spell constructor or deserialization rules.
            return new Spell()
            {
                Name = "New Spell",
                Level = "1",
                Range = "",
                Duration = "",
                Description = new System.Windows.Documents.FlowDocument(),
                Ritual = false,
                Classes = new ObservableCollection<string>(),
                Components = new Components()
                {
                    Verbal = false,
                    Somatic = false,
                    Material = false,
                    Raw = new List<string>()
                },
                School = Spell.SchoolOfMagic.Evocation,
                CastingTime = CastingTime.Action
            };
        }
    }
}
