using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(MonsterGroup), "monsterGroup")]
    [JsonDerivedType(typeof(EncounterEvent), "encounterEvent")]
    public partial class EncounterItem : ObservableObject
    {
        [ObservableProperty] public string name = "New Encounter Item";
        [ObservableProperty] public int roundNumber = 0;
        [ObservableProperty] public string description = string.Empty;

        public string QuantityDisplay => this is MonsterGroup mg ? mg.Quantity.ToString() : "-";
        public string TypeName
        {
            get
            {
                return this switch
                {
                    MonsterGroup => "Monster",
                    EncounterEvent => "Event",
                    _ => "Item"
                };
            }
        }

    }
}
