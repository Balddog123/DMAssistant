using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.ViewModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;

namespace DMAssistant.ViewModel
{
    public class EncounterViewModel : ObservableObject
    {
        private readonly Encounter _encounter;
        public Encounter Encounter => _encounter;

        public string Name
        {
            get => _encounter.Name;
            set => SetProperty(_encounter.Name, value, _encounter, (e, v) => e.Name = v);
        }

        public int TotalCR => EncounterItems.Sum(vm => vm.TotalCR);
        public int TotalXP => EncounterItems.Sum(vm => vm.TotalXP);
        public int TotalXPAvailable
        {
            get
            {
                int total = 0;
                foreach(PlayerCharacter pc in App.CampaignStore.CurrentCampaign.PCs)
                {
                    total += EncounterViewModel.AdventuringDayXP[pc.Level];
                }
                return total;
            }
        }
        public string Difficulty
        {
            get
            {
                var xp = AdjustedXp;
                var t = PartyThresholds;

                if (xp < t.Easy) return "Trivial";
                if (xp < t.Medium) return "Easy";
                if (xp < t.Hard) return "Medium";
                if (xp < t.Deadly) return "Hard";
                return "Deadly";
            }
        }

        public int AdjustedXp => (int)(TotalXP * MonsterCountMultiplier);

        private double MonsterCountMultiplier
        {
            get
            {
                int count = EncounterItems
                    .Where(i => i.EncounterItem is MonsterGroup mg)
                    .Sum(i => ((MonsterGroup)i.EncounterItem).Quantity);

                return count switch
                {
                    1 => 1.0,
                    2 => 1.5,
                    >= 3 and <= 6 => 2.0,
                    >= 7 and <= 10 => 2.5,
                    >= 11 and <= 14 => 3.0,
                    _ => 4.0
                };
            }
        }
        private static readonly Dictionary<int, (int Easy, int Medium, int Hard, int Deadly)> LevelThresholds
            = new()
        {
            { 0,  (15,   25,   50,   75) },
            { 1,  (25,   50,   75,   100) },
            { 2,  (50,   100,  150,  200) },
            { 3,  (75,   150,  225,  400) },
            { 4,  (125,  250,  375,  500) },
            { 5,  (250,  500,  750,  1100) },
            { 6,  (300,  600,  900,  1400) },
            { 7,  (350,  750,  1100, 1700) },
            { 8,  (450,  900,  1400, 2100) },
            { 9,  (550,  1100, 1600, 2400) },
            { 10, (600,  1200, 1900, 2800) },
            { 11, (800,  1600, 2400, 3600) },
            { 12, (1000, 2000, 3000, 4500) },
            { 13, (1100, 2200, 3400, 5100) },
            { 14, (1250, 2500, 3800, 5700) },
            { 15, (1400, 2800, 4300, 6400) },
            { 16, (1600, 3200, 4800, 7200) },
            { 17, (2000, 3900, 5900, 8800) },
            { 18, (2100, 4200, 6300, 9500) },
            { 19, (2400, 4900, 7300, 10900) },
            { 20, (2800, 5700, 8500, 12700) },
        };
        public (int Easy, int Medium, int Hard, int Deadly) PartyThresholds
        {
            get
            {
                int easy = 0, medium = 0, hard = 0, deadly = 0;

                foreach (PlayerCharacter pc in App.CampaignStore.CurrentCampaign.PCs)
                {
                    var t = LevelThresholds[pc.Level];
                    easy += t.Easy;
                    medium += t.Medium;
                    hard += t.Hard;
                    deadly += t.Deadly;
                }

                return (easy, medium, hard, deadly);
            }
        }
        private static Dictionary<int, int> AdventuringDayXP = new Dictionary<int, int>
        {
            { 0, 150 },
            { 1, 300 },
            { 2, 600 },
            { 3, 1200 },
            { 4, 1700 },
            { 5, 3500 },
            { 6, 4000 },
            { 7, 5000 },
            { 8, 6000 },
            { 9, 7500 },
            { 10, 9000 },
            { 11, 10500 },
            { 12, 11500 },
            { 13, 13500 },
            { 14, 15000 },
            { 15, 18000 },
            { 16, 20000 },
            { 17, 25000 },
            { 18, 27000 },
            { 19, 30000 },
            { 20, 40000 },
        };



        public ObservableCollection<EncounterItemViewModel> EncounterItems { get; }

        private EncounterItemViewModel _selectedItem;
        public EncounterItemViewModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        private CombatTrackerViewModel _currentCombat;
        public CombatTrackerViewModel CurrentCombat
        {
            get => _currentCombat;
            set => SetProperty(ref _currentCombat, value);
        }

        public ICommand AddMonsterCommand { get; }
        public ICommand AddEventCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand RunEncounterCommand { get; }


        public EncounterViewModel(Encounter encounter)
        {
            _encounter = encounter;

            // ---- Convert raw model items → ViewModels ----
            EncounterItems = new ObservableCollection<EncounterItemViewModel>(
                encounter.EncounterItems.Select(item => CreateItemViewModel(item))
            );

            // Keep parent in sync when items are added/removed
            EncounterItems.CollectionChanged += OnCollectionChanged;

            AddMonsterCommand = new RelayCommand(AddNewMonster);
            AddEventCommand = new RelayCommand(AddNewEvent);
            RemoveItemCommand = new RelayCommand<EncounterItemViewModel>(RemoveEncounterItem);
            RunEncounterCommand = new RelayCommand(RunEncounter);

            // Load combat
            if (encounter.CombatItems.Count > 0)
            {
                CurrentCombat = new CombatTrackerViewModel(Encounter, Encounter.CombatItems);
            }
        }


        // ------------------------------------------------------
        // Factory method ensures events are wired consistently
        // ------------------------------------------------------
        private EncounterItemViewModel CreateItemViewModel(EncounterItem item)
        {
            var vm = new EncounterItemViewModel(item);
            vm.MonsterChanged += OnChildItemChanged;
            return vm;
        }


        // ------------------------------------------------------
        // Recalculate TotalCR when a child updates
        // ------------------------------------------------------
        private void OnChildItemChanged()
        {
            OnPropertyChanged(nameof(TotalCR));
            OnPropertyChanged(nameof(TotalXP));
            OnPropertyChanged(nameof(Difficulty));
        }


        // ------------------------------------------------------
        // Keep model + VM collections in sync
        // ------------------------------------------------------
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Added
            if (e.NewItems != null)
            {
                foreach (EncounterItemViewModel vm in e.NewItems)
                {
                    vm.MonsterChanged += OnChildItemChanged;
                    _encounter.EncounterItems.Add(vm.EncounterItem);
                }
            }

            // Removed
            if (e.OldItems != null)
            {
                foreach (EncounterItemViewModel vm in e.OldItems)
                {
                    vm.MonsterChanged -= OnChildItemChanged;
                    _encounter.EncounterItems.Remove(vm.EncounterItem);
                }
            }

            OnPropertyChanged(nameof(TotalCR));
            OnPropertyChanged(nameof(TotalXP));
            OnPropertyChanged(nameof(Difficulty));
        }


        // ------------------------------------------------------
        // Commands
        // ------------------------------------------------------
        private void AddNewMonster()
        {
            var mg = new MonsterGroup();
            var vm = CreateItemViewModel(mg);
            EncounterItems.Add(vm);
        }

        private void AddNewEvent()
        {
            var ev = new EncounterEvent();
            var vm = CreateItemViewModel(ev);
            EncounterItems.Add(vm);
        }

        private void RemoveEncounterItem(EncounterItemViewModel vm)
        {
            if (vm != null)
                EncounterItems.Remove(vm);
        }

        private void RunEncounter()
        {
            Encounter.CurrentRound = 0;
            CurrentCombat = new CombatTrackerViewModel(Encounter, null);
        }
    }
}