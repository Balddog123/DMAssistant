using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.Repository;
using DMAssistant.View;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DMAssistant.ViewModel
{
    public class MonsterPanelViewModel : ObservableObject
    {
        public ICollectionView MonsterView { get; }
        public ICollectionView TypesView { get; }
        public ICollectionView CRView { get; }
        public Session _session { get; private set; }
        public ObservableCollection<Monster> AllMonsters { get; private set; }
        public ObservableCollection<string> AllCreatureTypes { get; private set; }
        public ObservableCollection<string> AllCRs { get; private set; }

        // This is the list of IDs stored in the Session
        private readonly ObservableCollection<string> _sessionMonsterIds;

        private Monster _selectedMonster;
        public Monster SelectedMonster
        {
            get => _selectedMonster;
            set
            {
                if (SetProperty(ref _selectedMonster, value))
                {
                    MonsterViewModel vm = new MonsterViewModel(_selectedMonster);
                    HookItemEvents(vm);
                    SelectedMonsterViewModel = vm;
                }
                    
            }
        }

        private string _search = ""; 
        public string Search 
        { 
            get => _search; 
            set 
            { 
                if (SetProperty(ref _search, value)) ApplyFilters(); 
            } 
        }
        private string _typeFilter = "All Types";
        public string TypeFilter
        {
            get => _typeFilter;
            set
            {
                if (SetProperty(ref _typeFilter, value)) ApplyFilters();
            }
        }
        private string _crFilter = "All CRs";
        public string CRFilter
        {
            get => _crFilter;
            set
            {
                if (SetProperty(ref _crFilter, value)) ApplyFilters();
            }
        }

        private MonsterViewModel _selectedMonsterViewModel;
        public MonsterViewModel SelectedMonsterViewModel
        {
            get => _selectedMonsterViewModel;
            set => SetProperty(ref _selectedMonsterViewModel, value);
        }

        public RelayCommand AddNewMonsterCommand { get; }
        public RelayCommand AddExistingMonsterCommand { get; }
        public IRelayCommand DeleteMonster => new RelayCommand<Monster>(monsterToDelete =>
        {
            if (MessageBox.Show($"Delete {monsterToDelete.Name}?",
                                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.CampaignStore.DeleteMonster(monsterToDelete, _session);
            }
        });
        public RelayCommand<Monster> DuplicateMonster { get; }

        public MonsterPanelViewModel(ObservableCollection<string> monsterIds, Session session)
        {
            _sessionMonsterIds = monsterIds;

            AllMonsters = new ObservableCollection<Monster>();
            AllCreatureTypes = new ObservableCollection<string>();
            AllCreatureTypes.Add("All Types");
            AllCRs = new ObservableCollection<string>();
            AllCRs.Add("All CRs");

            // Hydrate real Monster objects
            foreach (string id in monsterIds)
            {
                if (App.CampaignStore.MonsterIndex.TryGetValue(id, out var monster))
                    AllMonsters.Add(monster);
            }

            MonsterView = CollectionViewSource.GetDefaultView(AllMonsters);
            MonsterView.SortDescriptions.Add(new SortDescription(nameof(Monster.IsNew), ListSortDirection.Descending));
            MonsterView.SortDescriptions.Add(new SortDescription(nameof(Monster.Name), ListSortDirection.Ascending));
            MonsterView.Filter = FilterMonster;
            TypesView = CollectionViewSource.GetDefaultView(AllCreatureTypes);
            if (TypesView is ListCollectionView typesListView)
            {
                typesListView.SortDescriptions.Clear();
                typesListView.CustomSort = Comparer<string>.Create((a, b) =>
                {
                    if (a == "All Types") return -1;
                    return a.CompareTo(b);
                });
            }
            CRView = CollectionViewSource.GetDefaultView(AllCRs);
            if(CRView is ListCollectionView listView)
            {
                listView.SortDescriptions.Clear();
                listView.CustomSort = Comparer<string>.Create((a, b) =>
                {
                    if (a == "All CRs") return -1;
                    double da = ParseNumericValue(a);
                    double db = ParseNumericValue(b);

                    return da.CompareTo(db);
                });
            }
            ApplyFilters();

            FillCreatureTypes();
            FillCRs();

            AddNewMonsterCommand = new RelayCommand(() => AddNewMonster());
            AddExistingMonsterCommand = new RelayCommand(AddExistingMonster);
            DuplicateMonster = new RelayCommand<Monster>(monsterToDup =>
            {
                AddNewMonster(monsterToDup);
            });

            App.CampaignStore.MonsterDeleted += OnMonsterDeleted;
            _session = session;
        }

        private void HookItemEvents(MonsterViewModel vm)
        {
            vm.PropertyChanged += (_, args) =>
            {
                // Example: react to name changes
                if (args.PropertyName == nameof(MonsterViewModel.Name))
                {
                    // Raise panel-level update (e.g., refresh list)
                    OnPropertyChanged(nameof(AllMonsters));
                    ApplyFilters();
                }else if (args.PropertyName == nameof(MonsterViewModel.Meta))
                {
                    FillCreatureTypes();
                    OnPropertyChanged(nameof(AllCreatureTypes));
                    ApplyFilters();
                }
                else if (args.PropertyName == nameof(MonsterViewModel.Challenge))
                {
                    FillCRs();
                    OnPropertyChanged(nameof(AllCRs));
                    ApplyFilters();
                }
            };
        }

        double ParseNumericValue(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            // Handle fraction format like "1/4"
            if (input.Contains("/"))
            {
                var parts = input.Split('/');

                if (parts.Length == 2 &&
                    double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double numerator) &&
                    double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double denominator) &&
                    denominator != 0)
                {
                    return numerator / denominator;
                }
            }

            // Handle normal numbers like "2" or "0.5"
            if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                return value;

            // Fallback for invalid values
            return double.MinValue;
        }
        private void FillCreatureTypes()
        {
            foreach (var monster in AllMonsters)
            {
                string creatureType = ExtractType(monster.Meta);
                if (!AllCreatureTypes.Contains(creatureType))
                {
                    AllCreatureTypes.Add(creatureType);
                }
            }
        }
        private void FillCRs()
        {
            foreach (var monster in AllMonsters)
            {
                string cr = ExtractCR(monster.Challenge);
                if (!AllCRs.Contains(cr))
                {
                    AllCRs.Add(cr);
                }
            }
        }

        private void OnMonsterDeleted(Monster monster)
        {
            if (AllMonsters.Contains(monster)) AllMonsters.Remove(monster);

            if (SelectedMonster == monster) SelectedMonster = AllMonsters.FirstOrDefault();
        }
        private void AddExistingMonster()
        {
            var available = App.CampaignStore.CurrentCampaign.Monsters.ToList();

            if (!available.Any())
            {
                MessageBox.Show("No Monsters available!");
                return;
            }

            var window = new SelectMonsterWindow(available);
            if (window.ShowDialog() == true && window.SelectedMonster != null)
            {
                var monster = window.SelectedMonster;

                // Store only ID in session
                _sessionMonsterIds.Add(monster.ID);

                // Store real object in collection
                AllMonsters.Add(monster);

                SelectedMonster = monster;
            }
        }

        private void AddNewMonster(Monster monsterToCopy = null)
        {
            Monster newMonster;

            if (monsterToCopy == null) newMonster = new Monster() { IsNew = true };
            else newMonster = new Monster(monsterToCopy);

            // Monster belongs to global campaign list
            //App.CampaignStore.CurrentCampaign.Monsters.Add(newMonster);
            int index = App.CampaignStore.CurrentCampaign.Monsters.IndexOf(monsterToCopy);
            App.CampaignStore.CurrentCampaign.Monsters.Insert(index + 1, newMonster);
            App.CampaignStore.MonsterIndex[newMonster.ID] = newMonster;

            // Add ID to session
            _sessionMonsterIds.Add(newMonster.ID);

            // Add live object to panel
            //AllMonsters.Add(newMonster);
            AllMonsters.Insert(AllMonsters.IndexOf(monsterToCopy) + 1, newMonster);

            SelectedMonster = newMonster;
        }


        private string ExtractType(string meta)
        {
            // "Large aberration, lawful evil" → "aberration"
            var parts = meta.Split(',');
            if (parts.Length > 0)
            {
                var words = parts[0].Trim().Split(' ');
                string extractedType = words.Last();
                
                return char.ToUpper(extractedType[0]) + extractedType.Substring(1);
            }
            return "Unknown";
        }

        private string ExtractCR(string challenge)
        {
            // "10 (5,900 XP)" → "10"
            return challenge.Split(' ').First();
        }

        private bool FilterMonster(object obj)
        {
            if (obj is not Monster m) return false;

            if (!string.IsNullOrWhiteSpace(Search) &&
                !m.Name.Contains(Search, StringComparison.OrdinalIgnoreCase))
                return false;

            if (TypeFilter != "All Types" && ExtractType(m.Meta) != TypeFilter)
                return false;

            if (CRFilter != "All CRs" && ExtractCR(m.Challenge) != CRFilter)
                return false;

            return true;
        }

        private void ApplyFilters()
        {
            MonsterView.Refresh();
        }

        public void SelectMonster(Monster monster)
        {
            SelectedMonster = monster;
        }
    }
}
