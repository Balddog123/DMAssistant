using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DMAssistant.Model
{
    public enum CastingTime
    {
        Action,
        BonusAction,
        Reaction
    }

    public partial class Spell : ObservableObject
    {        
        [ObservableProperty] public CastingTime castingTime;
        public ObservableCollection<string> Classes { get; set; } = new ObservableCollection<string>();
        [ObservableProperty] public Components components;
        private FlowDocument description = new FlowDocument();
        [JsonConverter(typeof(FlowDocumentJsonConverter))]
        public FlowDocument Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }
        [ObservableProperty] public string duration;
        [ObservableProperty] public string level;
        [ObservableProperty] public string name;
        [ObservableProperty] public string range;
        [ObservableProperty] public bool ritual;
        public enum SchoolOfMagic
        {
            Abjuration,
            Conjuration,
            Divination,
            Enchantment,
            Evocation,
            Illusion,
            Necromancy,
            Transmutation
        }
        [ObservableProperty] public SchoolOfMagic school;

        public Spell() { }
    }

    public partial class Components : ObservableObject
    {
        [ObservableProperty] public bool material;
        [ObservableProperty] public List<string> raw;
        [ObservableProperty] public bool somatic;
        [ObservableProperty] public bool verbal;
    }
}
