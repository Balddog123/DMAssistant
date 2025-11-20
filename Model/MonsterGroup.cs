using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public partial class MonsterGroup : EncounterItem
    {
        [ObservableProperty] public int quantity = 1;
        [ObservableProperty] public string monsterId = string.Empty;

        partial void OnQuantityChanged(int oldValue, int newValue)
        {
            OnPropertyChanged(nameof(QuantityDisplay));
        }
    }
}
