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


        [JsonIgnore] public string QuantityDisplay => this is MonsterGroup mg ? mg.Quantity.ToString() : "-";
        [JsonIgnore] public int TotalCR
        {
            get
            {
                int cr = 0;
                if (this is MonsterGroup mg && App.CampaignStore.MonsterIndex.TryGetValue(mg.MonsterId, out Monster? m) && int.TryParse(m.Challenge.Split(" ")[0], out int challenge))
                {
                    cr += challenge * mg.Quantity;
                }
                return cr;
            }
        }
        [JsonIgnore] public string TypeName
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
