using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json.Serialization;

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
        [ObservableProperty] public int amount;
        [JsonIgnore, ObservableProperty] public int currentAmount;

        [JsonIgnore, ObservableProperty] private string currentHPInput;
        private bool _suppressHpInputProcessing;
        
        partial void OnCurrentHPInputChanged(string value)
        {
            if (_suppressHpInputProcessing)
                return;

            if (string.IsNullOrWhiteSpace(value))
                return;

            if (!TryParseHP(value, out int newHP))
                return;

            SetCurrentHP(newHP);
        }

        [JsonIgnore, ObservableProperty] public string amountDisplay;
        [ObservableProperty] public int maxHP;
        [ObservableProperty] public int armorClass;
        [ObservableProperty] public string encounterItemName;
        [ObservableProperty] public string encounterItemId;
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

        public CombatItem(string name, int initiative, int currentHP, int maxhp, int armorClass, int amount, Type combatItemType, string encounterItemName, string encounterItemId)
        {
            this.name = name;
            this.initiative = initiative;
            this.currentHP = currentHP;
            CurrentHPInput = CurrentHP.ToString();
            this.maxHP = maxhp;
            this.combatItemType = combatItemType;
            this.armorClass = armorClass;
            this.encounterItemName = encounterItemName;
            this.amount = amount;
            CurrentAmount = amount;

            AmountDisplay = Amount + "/" + Amount;
            this.encounterItemId = encounterItemId;
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

        public void SetCurrentHP(int newHP)
        {
            CurrentHP = Math.Max(newHP, 0);
            RecalculateAmounts();
            SyncHpInput();
        }
        private void RecalculateAmounts()
        {
            if (Amount <= 0 || MaxHP <= 0)
            {
                CurrentAmount = 0;
                AmountDisplay = $"0/{Amount}";
                return;
            }

            int hpPerUnit = MaxHP / Amount;

            int remainingUnits = Math.Max(CurrentHP / hpPerUnit, 0);
            if (CurrentHP % hpPerUnit != 0)
                remainingUnits++;

            CurrentAmount = remainingUnits;
            AmountDisplay = $"{CurrentAmount}/{Amount}";
        }
        private void SyncHpInput()
        {
            _suppressHpInputProcessing = true;
            CurrentHPInput = CurrentHP.ToString();
            _suppressHpInputProcessing = false;
        }
        public void Update(int maxHpPerUnit, int quantity, int damageAlreadyDealt)
        {
            Amount = quantity;
            MaxHP = maxHpPerUnit * quantity;

            int newHP = Math.Max(MaxHP - damageAlreadyDealt, 0);
            Debug.WriteLine($"maxHPPerUnit: {maxHpPerUnit}, quantity: {quantity}, damageAlreadyDealt: {damageAlreadyDealt}, newHP: {newHP}");

            SetCurrentHP(newHP);
        }


    }

}