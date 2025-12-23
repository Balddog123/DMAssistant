using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using System.Windows.Input;

namespace DMAssistant.ViewModel
{
    public class NPCViewModel : ObservableObject
    {
        public NPC NPC { get; private set; }
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
                NPC.Description = new FlowDocument();
                NPC.Description.Blocks.Add(new Paragraph(new Run(NPC.GetRandomDescription())));
            });
            RandomizeGoalCommand = new RelayCommand(() =>
            {
                NPC.Goal = new FlowDocument();
                NPC.Goal.Blocks.Add(new Paragraph(new Run(NPC.GetRandomGoal())));
            });
            RandomizeHomeCommand = new RelayCommand(() => NPC.Home = NPC.GetRandomLocation());
        }
    }

}
