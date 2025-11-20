using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public class Campaign
    {
        public ObservableCollection<Session> Sessions { get; set; } = new();
        public ObservableCollection<NPC> NPCs { get; set; } = new();
        public ObservableCollection<Item> Items { get; set; } = new();
        public ObservableCollection<Location> Locations { get; set; } = new();
        public ObservableCollection<Monster> Monsters { get; set; } = new();
        public ObservableCollection<PlayerCharacter> PCs { get; set; } = new();

        public string Notes { get; set; } = "";
        public string Fronts { get; set; } = "";
        public string Lore { get; set; } = "";

        public Map WorldMap { get; set; } = new Map();

        public void AddNPC(NPC npc)
        {
            NPCs.Add(npc);
        }

        public void SetMonsters(ObservableCollection<Monster> monsters)
        {
            Monsters = monsters;
        }

        public string GetAggregateSessionNotes()
        {
            string accumulated = "";
            for(int i = 0; i < Sessions.Count; i++)
            {
                accumulated += $"--------{i + 1} : {Sessions[i].Name}--------\n";
                accumulated += Sessions[i].Notes;
                accumulated += "\n\n\n";
            }
            return accumulated;
        }
    }

}
