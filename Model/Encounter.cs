using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public partial class Encounter : ObservableObject
    {
        [ObservableProperty] public string name = "New Encounter";
        private ObservableCollection<EncounterItem> _encounterItems = new ObservableCollection<EncounterItem>();
        public ObservableCollection<EncounterItem> EncounterItems { get => _encounterItems; set { SetProperty(ref  _encounterItems, value); } }
        private ObservableCollection<CombatItem> _combatItems = new();
        public ObservableCollection<CombatItem> CombatItems { get => _combatItems; set { SetProperty(ref _combatItems, value); } }
        [ObservableProperty] public int currentRound = 0;
    }
}
