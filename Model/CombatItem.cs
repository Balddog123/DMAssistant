using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Navigation;

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
        [JsonIgnore, ObservableProperty] public string currentHPInput;
        partial void OnCurrentHPInputChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || _suppressHpInputProcessing) return;
            _suppressHpInputProcessing = true;
            if(TryParseHP(value, out int newHP))
            {
                CurrentHP = newHP;
                CurrentHPInput = newHP.ToString();
            }
            _suppressHpInputProcessing = false;
        }
        private bool _suppressHpInputProcessing;
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

        public CombatItem(string name, int initiative, int currentHP, int maxhp, int armorClass, Type combatItemType, string encounterItemName)
        {
            this.name = name;
            this.initiative = initiative;
            this.currentHP = currentHP;
            currentHPInput = CurrentHP.ToString();
            this.maxHP = maxhp;
            this.combatItemType = combatItemType;
            this.armorClass = armorClass;
            this.encounterItemName = encounterItemName;
        }

        private bool TryParseHP(string input, out int result)
        {
            result = CurrentHP;
            input = input.Trim();

            if (input.Contains('-'))
            {
                string[] split = input.Split('-', StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 2 && int.TryParse(split[0], out int a) && int.TryParse(split[1], out int b))
                {
                    result = a - b;
                    return true;
                }
            }
            else if (input.Contains("+"))
            {
                string[] split = input.Split('+', StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 2 && int.TryParse(split[0], out int a) && int.TryParse(split[1], out int b))
                {
                    result = a + b;
                    return true;
                }
            }
            else if (input.Contains("*"))
            {
                string[] split = input.Split('*', StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 2 && int.TryParse(split[0], out int a) && int.TryParse(split[1], out int b))
                {
                    result = a * b;
                    return true;
                }
            }
            else if (input.Contains("/"))
            {
                string[] split = input.Split('/', StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 2 && int.TryParse(split[0], out int a) && int.TryParse(split[1], out int b))
                {
                    result = a / b;
                    return true;
                }
            }

            return int.TryParse(input, out result);
        }

        

    }

}
