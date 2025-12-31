using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DMAssistant.Model
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(MonsterGroup), "monsterGroup")]
    [JsonDerivedType(typeof(EncounterEvent), "encounterEvent")]
    public partial class EncounterItem : ObservableObject
    {
        public string id { get; } = Guid.NewGuid().ToString();
        [ObservableProperty] public string name = "";
        [ObservableProperty] public int roundNumber = 0;
        [ObservableProperty] public int initialInitiative = 0;

        private FlowDocument description = new FlowDocument();
        [JsonConverter(typeof(FlowDocumentJsonConverter))]
        public FlowDocument Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        [JsonIgnore] public string QuantityDisplay => this is MonsterGroup mg ? mg.Quantity.ToString() : "-";
        [JsonIgnore] public string CR
        {
            get
            {
                string cr = "0";
                if (this is MonsterGroup mg && App.CampaignStore.MonsterIndex.TryGetValue(mg.MonsterId, out Monster? m))
                {
                    cr = m.Challenge.Split(" ")[0];
                }
                return cr;
            }
        }
        [JsonIgnore]
        public int XP
        {
            get
            {
                int cr = 0;
                
                if (this is MonsterGroup mg && App.CampaignStore.MonsterIndex.TryGetValue(mg.MonsterId, out Monster? m))
                {
                    var match = Regex.Match(m.Challenge, @"\(([\d,]+) XP\)");
                    //int.TryParse(m.Challenge.Split(" ")[1].Substring(1), out int XP)
                    if (match.Success)
                    {
                        cr = int.Parse(match.Groups[1].Value.Replace(",", ""));
                    }
                    
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

        public EncounterItem()
        {

        }

        public EncounterItem(EncounterItem itemToCopy)
        {
            Name = itemToCopy.Name;
            Description = itemToCopy.Description;
            RoundNumber = itemToCopy.RoundNumber;
            InitialInitiative = itemToCopy.InitialInitiative;
            
        }
        public virtual EncounterItem Clone()
        {
            return new EncounterItem(this);
        }
    }
}
