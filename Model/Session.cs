using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
    }
}
