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
        public RelayCommand ShowLoreCommand { get; }
        public RelayCommand ShowPCsCommand { get; }
        public RelayCommand ShowFrontsCommand { get; }
        public RelayCommand ShowLocationsCommand { get; }
        public RelayCommand ShowWorldMapCommand { get; }
        public RelayCommand ShowCampaignDetails { get; }
        public CampaignViewModel()
        {
            AccumulateIds();
            ShowCampaignDetails = new RelayCommand(() => CurrentModuleView = new CampaignDetailsViewModel());
            ShowNPCsCommand = new RelayCommand(() => CurrentModuleView = new NPCPanelViewModel(_npcIds, null));
            ShowItemsCommand = new RelayCommand(() => CurrentModuleView = new ItemPanelViewModel(_itemIds, null));
            ShowMonstersCommand = new RelayCommand(() => CurrentModuleView = new MonsterPanelViewModel(_monsterIds, null));
            ShowLocationsCommand = new RelayCommand(() => CurrentModuleView = new LocationPanelViewModel(_locationIds, null));

            ShowNotesCommand = new RelayCommand(() => CurrentModuleView = new CampaignNotesViewModel());
            
            ShowLoreCommand = new RelayCommand(() => CurrentModuleView = new CampaignLoreViewModel());
            ShowPCsCommand = new RelayCommand(() => CurrentModuleView = new CampaignPCViewModel());
            ShowFrontsCommand = new RelayCommand(() => CurrentModuleView = new CampaignFrontsViewModel());

            ShowWorldMapCommand = new RelayCommand(() => CurrentModuleView = new WorldMapViewModel());
            // Default module
            //CurrentModuleView = new NPCPanelViewModel(_npcIds);
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
