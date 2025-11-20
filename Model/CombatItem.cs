using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public enum Condition
    {
        None,
        Blinded,
        Charmed,
        Deafened,
        Exhaustion,
        Frightened,
        Grappled,
        Incapacitated,
        Invisible,
        Paralyzed,
        Petrified,
        Poisoned,
        Prone,
        Restrained,
        Stunned,
        Unconscious
    }

    public partial class CombatItem : ObservableObject
    {
        [ObservableProperty] public string name;
        [ObservableProperty] public int initiative;
        [ObservableProperty] public int currentHP;
        [ObservableProperty] public int maxHP;
        [ObservableProperty] public int armorClass;
        [ObservableProperty] public string encounterItemName;
        private ObservableCollection<Condition> _conditions = new();
        public ObservableCollection<Condition> Conditions
        {
            get => _conditions;
            set
            {
                SetProperty(ref _conditions, value);
            }
        }
        public enum Type
        {
            Player,
            Monster,
            Event
        }
        [ObservableProperty] public Type combatItemType;

        public CombatItem(string name, int initiative, int hp, int maxhp, int armorClass, Type combatItemType, string encounterItemName)
        {
            this.name = name;
            this.initiative = initiative;
            currentHP = hp;
            maxHP = maxhp;
            this.combatItemType = combatItemType;
            this.armorClass = armorClass;
            this.encounterItemName = encounterItemName;
        }

    }

}
