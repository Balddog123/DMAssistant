using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DMAssistant.ViewModel
{
    public class NPCViewModel : ObservableObject
    {
        public NPC NPC { get; private set; }
        public List<string> AvailableRaces { get; } = new List<string>();

        public List<Location> AvailableLocations { get; } = new List<Location>();

        public NPCViewModel(NPC npc)
        {
            AvailableRaces = NPC.AvailableRaces.Keys.ToList();
            AvailableLocations = App.CampaignStore.CurrentCampaign.Locations.ToList();
            AvailableLocations.Insert(0, new Location{ Name = "None" });
            NPC = npc;
        }
    }

}
