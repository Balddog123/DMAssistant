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
        [ObservableProperty] public int numberOfGroups = 1;
        [ObservableProperty] public bool isAlly = false;

        partial void OnMonsterIdChanged(string value)
        {
            OnPropertyChanged(nameof(CR));
        }
        partial void OnQuantityChanged(int oldValue, int newValue)
        {
            OnPropertyChanged(nameof(QuantityDisplay));
        }

        public MonsterGroup()
        {
            name = "New Monster Group";
        }
        protected MonsterGroup(MonsterGroup other) : base(other)
        {
            Quantity = other.Quantity;
            MonsterId = other.MonsterId;
            NumberOfGroups = other.NumberOfGroups;
            IsAlly = other.IsAlly;
        }

        public override EncounterItem Clone()
        {
            return new MonsterGroup(this);
        }


    }
}
