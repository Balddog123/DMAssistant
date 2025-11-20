using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.Repository;
using DMAssistant.View;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DMAssistant.ViewModel
{
    public class MonsterPanelViewModel : ObservableObject
    {
        public ICollectionView MonsterView { get; }
        public Session _session { get; private set; }
        public ObservableCollection<Monster> AllMonsters { get; private set; }

        // This is the list of IDs stored in the Session
        private readonly ObservableCollection<string> _sessionMonsterIds;

        private Monster _selectedMonster;
        public Monster SelectedMonster
        {
            get => _selectedMonster;
            set
            {
                if (SetProperty(ref _selectedMonster, value))
                    SelectedMonsterViewModel = new MonsterViewModel(_selectedMonster)
                    {
                        DeleteCommand = DeleteMonsterCommand // parent VM command
                    };
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
        private string _typeFilter = "All";
        public string TypeFilter
        {
            get => _typeFilter;
            set
            {
                if (SetProperty(ref _typeFilter, value)) ApplyFilters();
            }
        }
        private string _crFilter = "All";
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
        public IRelayCommand DeleteMonsterCommand => new RelayCommand(async () =>
        {
            if (MessageBox.Show($"Delete {SelectedMonster.Name}?",
                                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var loc = SelectedMonster;

                // Clear the view model so bindings detach
                SelectedMonsterViewModel = null;
                SelectedMonster = null;

                // Allow UI to update before deletion
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    App.CampaignStore.DeleteMonster(loc, _session);
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        });

        public MonsterPanelViewModel(ObservableCollection<string> monsterIds, Session session)
        {
            _sessionMonsterIds = monsterIds;

            AllMonsters = new ObservableCollection<Monster>();

            // Hydrate real Monster objects
            foreach (string id in monsterIds)
            {
                if (App.CampaignStore.MonsterIndex.TryGetValue(id, out var monster))
                    AllMonsters.Add(monster);
            }

            MonsterView = CollectionViewSource.GetDefaultView(AllMonsters);
            MonsterView.Filter = FilterMonster;

            AddNewMonsterCommand = new RelayCommand(AddNewMonster);
            AddExistingMonsterCommand = new RelayCommand(AddExistingMonster);

            App.CampaignStore.MonsterDeleted += OnMonsterDeleted;
            _session = session;
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

        private void AddNewMonster()
        {
            var newMonster = new Monster();

            // Monster belongs to global campaign list
            App.CampaignStore.CurrentCampaign.Monsters.Add(newMonster);
            App.CampaignStore.MonsterIndex[newMonster.ID] = newMonster;

            // Add ID to session
            _sessionMonsterIds.Add(newMonster.ID);

            // Add live object to panel
            AllMonsters.Add(newMonster);

            SelectedMonster = newMonster;
        }


        private string ExtractType(string meta)
        {
            // "Large aberration, lawful evil" → "aberration"
            var parts = meta.Split(',');
            if (parts.Length > 0)
            {
                var words = parts[0].Trim().Split(' ');
                return words.Last();
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

            if (TypeFilter != "All" && ExtractType(m.Meta) != TypeFilter)
                return false;

            if (CRFilter != "All" && ExtractCR(m.Challenge) != CRFilter)
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
