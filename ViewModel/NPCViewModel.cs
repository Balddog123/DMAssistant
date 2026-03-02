using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Input;

namespace DMAssistant.ViewModel
{
    public class NPCViewModel : ObservableObject
    {
        public NPC NPC { get; private set; }

        public string Name
        {
            get => NPC != null ? NPC.Name : string.Empty;
            set
            {
                if (IsNew) IsNew = false;
                SetProperty(NPC.Name, value, NPC, (m, v) => m.Name = v);
                OnPropertyChanged();
            }
        }
        private bool _isNew;
        public bool IsNew
        {
            get => _isNew;
            set => SetProperty(ref _isNew, value);
        }

        public List<string> AvailableRaces { get; } = new List<string>();

        public List<Location> AvailableLocations { get; } = new List<Location>();

        public ICommand RandomizeNameCommand { get; }
        public ICommand RandomizeRaceCommand { get; }
        public ICommand RandomizeDescriptionCommand { get; }
        public ICommand RandomizeGoalCommand { get; }
        public ICommand RandomizeHomeCommand { get; }


        public NPCViewModel(NPC npc)
        {
            AvailableRaces = NPC.AvailableRaces.Keys.ToList();
            AvailableLocations = App.CampaignStore.CurrentCampaign.Locations.ToList();
            AvailableLocations.Insert(0, new Location{ Name = "None" });
            NPC = npc;

            RandomizeNameCommand = new RelayCommand(() => NPC.Name = NPC.GetRandomName(NPC.Gender));
            RandomizeRaceCommand = new RelayCommand(() => NPC.Race = NPC.GetRandomRace());
            RandomizeDescriptionCommand = new RelayCommand(() =>
            {
                FlowDocument flowDoc = new FlowDocument();
                flowDoc.Blocks.Add(new Paragraph(new Run(NPC.GetRandomDescription())));
                NPC.Description = flowDoc;
            });
            RandomizeGoalCommand = new RelayCommand(() =>
            {
                Debug.WriteLine("Randomizing goal...");
                FlowDocument flowDoc = new FlowDocument();
                flowDoc.Blocks.Add(new Paragraph(new Run(NPC.GetRandomGoal())));
                NPC.Goal = flowDoc;
            });
            RandomizeHomeCommand = new RelayCommand(() => NPC.Home = NPC.GetRandomLocation());
        }
    }

}
