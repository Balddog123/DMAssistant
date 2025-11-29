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
            // Normal races (sorted alphabetically)
            "Aarakocra",
            "Dwarf",
            "Elf",
            "Half-Orc",
            "Halfling",
            "Human",
            "Orc",
            "Tiefling",

            // Monster Types (5e)
            "Aberration",
            "Beast",
            "Celestial",
            "Construct",
            "Dragon",
            "Elemental",
            "Fey",
            "Fiend",
            "Giant",
            "Humanoid",
            "Monstrosity",
            "Ooze",
            "Plant",
            "Undead"
        };


        public NPCViewModel(NPC npc)
        {
            NPC = npc;
        }
    }

}
