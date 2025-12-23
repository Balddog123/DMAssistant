using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DMAssistant.Model
{
    public class Monster : ObservableObject
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        private string _name = "New Monster";
        [JsonPropertyName("name")]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        private string _meta = "";
        [JsonPropertyName("meta")]
        public string Meta
        {
            get => _meta;
            set => SetProperty(ref _meta, value);
        }

        [JsonPropertyName("Armor Class")]
        public string ArmorClass { get; set; } = "";

        [JsonPropertyName("Hit Points")]
        public string HitPoints { get; set; } = "";

        [JsonPropertyName("Speed")]
        public string Speed { get; set; } = "";

        public string STR { get; set; } = "";
        public string STR_mod { get; set; } = "";

        public string DEX { get; set; } = "";
        public string DEX_mod { get; set; } = "";

        public string CON { get; set; } = "";
        public string CON_mod { get; set; } = "";

        public string INT { get; set; } = "";
        public string INT_mod { get; set; } = "";

        public string WIS { get; set; } = "";
        public string WIS_mod { get; set; } = "";

        public string CHA { get; set; } = "";
        public string CHA_mod { get; set; } = "";
        

        public string Skills { get; set; } = "";


        [JsonPropertyName("Saving Throws")] public string SavingThrows { get; set; } = "";
        [JsonPropertyName("Damage Resistances")] public string DamageResistances { get; set; } = "";
        [JsonPropertyName("Damage Immunities")] public string DamageImmunities { get; set; } = "";
        [JsonPropertyName("Condition Immunities")] public string ConditionImmunities { get; set; } = "";
        public string Senses { get; set; } = "";
        public string Languages { get; set; } = "";
        [JsonConverter(typeof(FlowDocumentJsonConverter))] public FlowDocument Traits { get; set; } = new FlowDocument();

        [JsonPropertyName("Challenge")]
        private string _challenge = "0";
        public string Challenge
        {
            get => _challenge;
            set => SetProperty(ref _challenge, value);
        }

        [JsonConverter(typeof(FlowDocumentJsonConverter))] public FlowDocument Actions { get; set; } = new FlowDocument();

        [JsonPropertyName("Legendary Actions"), JsonConverter(typeof(FlowDocumentJsonConverter))] public FlowDocument LegendaryActions { get; set; } = new FlowDocument();

        [JsonPropertyName("img_url")]
        public string ImageUrl { get; set; } = "";

        public static int GetMod(int stat)
        {
            return (stat - 10) / 2;
        }

        public Monster()
        {

        }
        public Monster(Monster other)
        {
            ID = Guid.NewGuid().ToString();
            Name = "Copy of " + other.Name;
            Meta = other.Meta;
            ArmorClass = other.ArmorClass;
            HitPoints = other.HitPoints;
            Speed = other.Speed;

            STR = other.STR;
            STR_mod = other.STR_mod;
            DEX = other.DEX;
            DEX_mod = other.DEX_mod;
            CON = other.CON;
            CON_mod = other.CON_mod;
            INT = other.INT;
            INT_mod = other.INT_mod;
            WIS = other.WIS;
            WIS_mod = other.WIS_mod;
            CHA = other.CHA;
            CHA_mod = other.CHA_mod;

            Skills = other.Skills;
            SavingThrows = other.SavingThrows;
            DamageResistances = other.DamageResistances;
            DamageImmunities = other.DamageImmunities;
            ConditionImmunities = other.ConditionImmunities;
            Senses = other.Senses;
            Languages = other.Languages;
            Traits = other.Traits;
            Challenge = other.Challenge;
            Actions = other.Actions;
            LegendaryActions = other.LegendaryActions;
            ImageUrl = other.ImageUrl;
        }

    }
}

