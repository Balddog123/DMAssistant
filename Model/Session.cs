using DMAssistant.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public class Session
    {
        public string Name { get; set; } = "New Session";
        public ObservableCollection<string> NPCIDs { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> ItemIDs { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<Encounter> Encounters { get; set; } = new ObservableCollection<Encounter>();
        public ObservableCollection<string> LocationIDs { get; set; } = new ObservableCollection<string>();
        public string Scenes { get; set; } = "Your Potential Scenes go here:\n1. First\n2. Second";
        public ObservableCollection<ChecklistItem> Secrets { get; set; } = new ObservableCollection<ChecklistItem>();
        public string Notes { get; set; } = "";

        [JsonIgnore] public NPCPanelViewModel NPCPanelViewModel { get; set; }
        [JsonIgnore] public ItemPanelViewModel ItemPanelViewModel { get; set; }
        [JsonIgnore] public EncountersPanelViewModel EncountersPanelViewModel { get; set; }
        [JsonIgnore] public LocationPanelViewModel LocationPanelViewModel { get; set; }

        /*
        NPCPanel = new NPCPanelViewModel(_selectedSession.NPCIDs, _selectedSession);
                    ItemPanel = new ItemPanelViewModel(_selectedSession.ItemIDs, _selectedSession);
                    EncountersPanel = new EncountersPanelViewModel(_selectedSession);
                    LocationPanel = new LocationPanelViewModel(_selectedSession.LocationIDs, _selectedSession);
         */
    }
}
