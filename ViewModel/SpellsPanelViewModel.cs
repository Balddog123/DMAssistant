using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
                App.CampaignStore.CurrentCampaign.Spells.Select(spell => CreateSpellViewModel(spell))
            );

            AddSpellCommand = new RelayCommand(() => SpellList.Add(CreateSpellViewModel(CreateDefaultSpell())));
            DeleteSpellCommand = new RelayCommand<SpellViewModel>(deleteSpell =>
            {
                if (MessageBox.Show($"Delete {deleteSpell.Spell.Name}?",
                                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    SpellList.Remove(deleteSpell);
                }
                
            });

            SpellsView = CollectionViewSource.GetDefaultView(SpellList);
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
        private SpellViewModel CreateSpellViewModel(Spell spell)
        {
            var vm = new SpellViewModel(spell);

            // listen to whenever Name/Rank/etc. changes
            HookSpellEvents(vm);

            return vm;
        }

        private void HookSpellEvents(SpellViewModel vm)
        {
            vm.PropertyChanged += (_, args) =>
            {
                // Example: react to name changes
                if (args.PropertyName == nameof(ItemViewModel.Name))
                {
                    // Raise panel-level update (e.g., refresh list)
                    OnPropertyChanged(nameof(SpellList));
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
