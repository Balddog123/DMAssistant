using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;

namespace DMAssistant.Model
{
    public class Campaign
    {
        public string Name { get; set; } = "Campaign-" + Guid.NewGuid().ToString();
        public ObservableCollection<Session> Sessions { get; set; } = new();
        public ObservableCollection<NPC> NPCs { get; set; } = new();
        public ObservableCollection<Item> Items { get; set; } = new();
        public ObservableCollection<Location> Locations { get; set; } = new();
        public ObservableCollection<Monster> Monsters { get; set; } = new();
        public ObservableCollection<PlayerCharacter> PCs { get; set; } = new();
        public ObservableCollection<Spell> Spells { get; set; } = new();
        public ObservableCollection<Table> Tables { get; set; } = new();

        public string Notes { get; set; } = "";
        public ObservableCollection<string> Fronts { get; set; } = new();
        public ObservableCollection<Lore> Lore { get; set; } = new();

        public Map WorldMap { get; set; } = new Map();

        public void AddNPC(NPC npc)
        {
            NPCs.Add(npc);
        }

        public void SetMonsters(ObservableCollection<Monster> monsters)
        {
            Monsters = monsters;
        }

        public FlowDocument GetAggregateSessionNotes()
        {
            FlowDocument result = new FlowDocument();

            for (int i = 0; i < Sessions.Count; i++)
            {
                Debug.WriteLine($"Adding session {i}...");

                foreach (Block block in Sessions[i].Notes.Blocks)
                {
                    Debug.WriteLine($"Cloning session {i} note: {block}");

                    Block clonedBlock = CloneBlock(block);
                    result.Blocks.Add(clonedBlock);
                }
            }

            return result;
        }
        private Block CloneBlock(Block block)
        {
            string xaml = XamlWriter.Save(block);

            using (StringReader stringReader = new StringReader(xaml))
            using (XmlReader xmlReader = XmlReader.Create(stringReader))
            {
                return (Block)XamlReader.Load(xmlReader);
            }
        }
    }

}
