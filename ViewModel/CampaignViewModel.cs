using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DMAssistant.ViewModel
{
    public class CampaignViewModel : ObservableObject
    {
        // Aggregated ID lists for the Campaign “overview” panels
        private readonly ObservableCollection<string> _npcIds = new();
        private readonly ObservableCollection<string> _itemIds = new();
        private readonly ObservableCollection<string> _monsterIds = new();
        private readonly ObservableCollection<string> _locationIds = new();
        
        private CampaignDetailsViewModel _campaignDetailsViewModel;
        private NPCPanelViewModel _npcPanelViewModel;
        private ItemPanelViewModel _itemPanelViewModel;
        private MonsterPanelViewModel _monsterPanelViewModel;
        private LocationPanelViewModel _locationPanelViewModel;
        private SpellsPanelViewModel _spellPanelViewModel;
        private TablePanelViewModel _tablePanelViewModel;
        private CampaignNotesViewModel _campaignNotesViewModel;
        private CampaignLoreViewModel _campaignLoreViewModel;
        private CampaignPCViewModel _campaignPCViewModel;
        private CampaignFrontsViewModel _campaignFrontsViewModel;
        private WorldMapViewModel _worldMapViewModel;
        private ShopViewModel _shopViewModel;
        private DiceRollCalculatorViewModel _diceRollCalculatorViewModel;
        private object _currentModuleView;
        public object CurrentModuleView
        {
            get => _currentModuleView;
            set => SetProperty(ref _currentModuleView, value);
        }

        public RelayCommand ShowNPCsCommand { get; }
        public RelayCommand ShowItemsCommand { get; }
        public RelayCommand ShowMonstersCommand { get; }
        public RelayCommand ShowNotesCommand { get; }
        public RelayCommand ShowTablesCommand { get; }
        public RelayCommand ShowLoreCommand { get; }
        public RelayCommand ShowPCsCommand { get; }
        public RelayCommand ShowFrontsCommand { get; }
        public RelayCommand ShowLocationsCommand { get; }
        public RelayCommand ShowWorldMapCommand { get; }
        public RelayCommand ShowCampaignDetails { get; }
        public RelayCommand ShowSpellsCommand { get; }
        public RelayCommand ShowShopCommand { get; }
        public RelayCommand ShowDiceRollCommand { get; }
        public CampaignViewModel()
        {
            AccumulateIds();
            ShowCampaignDetails = new RelayCommand(() =>ShowModule(ref _campaignDetailsViewModel,() => new CampaignDetailsViewModel()));

            ShowNPCsCommand = new RelayCommand(() =>ShowModule(ref _npcPanelViewModel,() => new NPCPanelViewModel(_npcIds, null)));

            ShowItemsCommand = new RelayCommand(() =>ShowModule(ref _itemPanelViewModel,() => new ItemPanelViewModel(new ObservableCollection<string>(App.CampaignStore.CurrentCampaign.Items.Select(i => i.ID)),null)));

            ShowMonstersCommand = new RelayCommand(() =>ShowModule(ref _monsterPanelViewModel,() => new MonsterPanelViewModel(_monsterIds, null)));

            ShowLocationsCommand = new RelayCommand(() =>ShowModule(ref _locationPanelViewModel,() => new LocationPanelViewModel(_locationIds, null)));

            ShowSpellsCommand = new RelayCommand(() =>ShowModule(ref _spellPanelViewModel,() => new SpellsPanelViewModel()));

            ShowNotesCommand = new RelayCommand(() =>ShowModule(ref _campaignNotesViewModel,() => new CampaignNotesViewModel()));

            ShowTablesCommand = new RelayCommand(() =>ShowModule(ref _tablePanelViewModel,() => new TablePanelViewModel()));

            ShowLoreCommand = new RelayCommand(() =>ShowModule(ref _campaignLoreViewModel,() => new CampaignLoreViewModel()));

            ShowPCsCommand = new RelayCommand(() => ShowModule(ref _campaignPCViewModel, () => new CampaignPCViewModel()));

            ShowFrontsCommand = new RelayCommand(() => ShowModule(ref _campaignFrontsViewModel, () => new CampaignFrontsViewModel()));

            ShowWorldMapCommand = new RelayCommand(() => ShowModule(ref _worldMapViewModel, () => new WorldMapViewModel()));

            ShowShopCommand = new RelayCommand(() =>
            {
                _shopViewModel = new ShopViewModel();
                CurrentModuleView = _shopViewModel;
            });
            ShowDiceRollCommand = new RelayCommand(() => ShowModule(ref _diceRollCalculatorViewModel, () => new DiceRollCalculatorViewModel()));
        }

        private void ShowModule<T>(ref T module, Func<T> factory) where T : class
        {
            module ??= factory();
            CurrentModuleView = module;
        }

        public void ResetView()
        {
            CurrentModuleView = null;
        }

        // Build a de-duplicated, campaign-wide list of IDs (root + all sessions)
        public void AccumulateIds()
        {
            _npcIds.Clear();
            _itemIds.Clear();
            _monsterIds.Clear();
            _locationIds.Clear();

            var npcSet = new HashSet<string>();
            var itemSet = new HashSet<string>();
            var monsterSet = new HashSet<string>();
            var locationSet = new HashSet<string>();

            var campaign = App.CampaignStore.CurrentCampaign;

            // Root campaign objects -> IDs (assuming each has an ID property)
            foreach (var n in campaign.NPCs) npcSet.Add(n.ID);
            foreach (var i in campaign.Items) itemSet.Add(i.ID);
            foreach (var m in campaign.Monsters) monsterSet.Add(m.ID);
            foreach (var l in campaign.Locations) locationSet.Add(l.ID);

            // Sessions already store IDs
            foreach (Session s in campaign.Sessions)
            {
                foreach (var id in s.NPCIDs) npcSet.Add(id);
                foreach (var id in s.ItemIDs) itemSet.Add(id);
                foreach (var id in s.LocationIDs) locationSet.Add(id);
            }

            foreach (var id in npcSet) _npcIds.Add(id);
            foreach (var id in itemSet) _itemIds.Add(id);
            foreach (var id in monsterSet) _monsterIds.Add(id);
            foreach (var id in locationSet) _locationIds.Add(id);
        }

    }
}
