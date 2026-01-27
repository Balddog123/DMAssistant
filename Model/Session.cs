using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.Helpers;
using DMAssistant.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DMAssistant.Model
{
    public class Session : ObservableObject
    {
        private string name = "New Session";
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }
        public ObservableCollection<string> NPCIDs { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> ItemIDs { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<Encounter> Encounters { get; set; } = new ObservableCollection<Encounter>();
        public ObservableCollection<string> LocationIDs { get; set; } = new ObservableCollection<string>();

        private FlowDocument scenes = new FlowDocument();
        [JsonConverter(typeof(FlowDocumentJsonConverter))]
        public FlowDocument Scenes
        {
            get => scenes;
            set => SetProperty(ref scenes, value);
        }

        public ObservableCollection<ChecklistItem> Secrets { get; set; } = new ObservableCollection<ChecklistItem>();

        private FlowDocument notes = new FlowDocument();
        [JsonConverter(typeof(FlowDocumentJsonConverter))]
        public FlowDocument Notes
        {
            get => notes;
            set => SetProperty(ref notes, value);
        }

        [JsonIgnore] public NPCPanelViewModel NPCPanelViewModel { get; set; }
        [JsonIgnore] public ItemPanelViewModel ItemPanelViewModel { get; set; }
        [JsonIgnore] public EncountersPanelViewModel EncountersPanelViewModel { get; set; }
        [JsonIgnore] public LocationPanelViewModel LocationPanelViewModel { get; set; }

        public Session()
        {

        }
    }
}
