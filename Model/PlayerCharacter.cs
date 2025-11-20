using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public partial class PlayerCharacter : ObservableObject
    {
        [ObservableProperty] public string name = string.Empty;
        [ObservableProperty] public string playerName = string.Empty;
        [ObservableProperty] public int hp = 0;
        [ObservableProperty] public int armorClass = 0;
        [ObservableProperty] public int passivePerseption = 0;
        [ObservableProperty] public ObservableCollection<Item> items = new();
        [ObservableProperty] public string backstory = string.Empty;
        [ObservableProperty] public string goal = string.Empty;
        [ObservableProperty] public string alliances = string.Empty;
        [ObservableProperty] public string enemies = string.Empty;
    }
}
