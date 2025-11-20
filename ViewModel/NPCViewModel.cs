using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DMAssistant.ViewModel
{
    public class NPCViewModel : ObservableObject
    {
        public NPC NPC { get; private set; }
        public List<string> AvailableRaces { get; } = new List<string>
        {
            "Human", "Elf", "Dwarf", "Half-Orc", "Aarakocra", "Orc", "Halfling", "Tiefling"
        };
        public IRelayCommand DeleteCommand { get; set; }

        public NPCViewModel(NPC npc)
        {
            NPC = npc;
        }
    }

}
