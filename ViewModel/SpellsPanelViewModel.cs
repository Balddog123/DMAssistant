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
        public ObservableCollection<string> spellLevels = new ObservableCollection<string>();
        public ICollectionView SpellsView { get; }
        public ICollectionView SpellLevelsView { get; }

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
        private string _selectedLevel = "All Levels";
        public string SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                if (SetProperty(ref _selectedLevel, value)) ApplyFilters();
            }
        }

        public IRelayCommand AddSpellCommand { get; }
        public IRelayCommand DeleteSpellCommand { get; }
        public SpellsPanelViewModel()
        {
            spellLevels.Add("All Levels");
            SpellList = new ObservableCollection<SpellViewModel>(
                App.CampaignStore.CurrentCampaign.Spells.Select(spell => CreateSpellViewModel(spell, false))
            );
            
            FillSpellLevels();

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
            SpellLevelsView = CollectionViewSource.GetDefaultView(spellLevels);
            if (SpellLevelsView is ListCollectionView listView)
            {
                listView.SortDescriptions.Clear();
                listView.CustomSort = Comparer<string>.Create((a, b) =>
                {
                    if (a == "All Levels") return -1;
                    if (b == "All Levels") return 1;
                    return a.CompareTo(b);
                });
            }
            ApplyFilters();
        }

        private void FillSpellLevels()
        {
            foreach (var spell in SpellList)
            {
                if(!spellLevels.Contains(spell.Level)) spellLevels.Add(spell.Level);
            }
        }
        private bool FilterSpell(object obj)
        {
            if (obj is not SpellViewModel m) return false;

            if (!string.IsNullOrWhiteSpace(Search) &&
                !m.Name.Contains(Search, StringComparison.OrdinalIgnoreCase))
                return false;

            if (SelectedLevel != "All Levels" && m.Level != SelectedLevel)
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
            return vm;
        }

        private void HookSpellEvents(SpellViewModel vm)
        {
            vm.PropertyChanged += (_, args) =>
            {
                // Example: react to name changes
                if (args.PropertyName == nameof(SpellViewModel.Name))
                {                    
                    // Raise panel-level update (e.g., refresh list)
                    OnPropertyChanged(nameof(SpellList));
                    ApplyFilters();
                }else if (args.PropertyName == nameof(SpellViewModel.Level))
                {
                    FillSpellLevels();
                    ApplyFilters();
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
