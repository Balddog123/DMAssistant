using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using static DMAssistant.Model.NameRow;

namespace DMAssistant.Model
{
    public partial class Item : ObservableObject
    {
        private static List<string> ItemTypes = new List<string>
    {
        "Statuette",
        "Totem",
        "Weapon",
        "Armor",
        "Jewelry",
        "Clothing",
        "Container",
        "Book",
        "Scroll",
        "Potion",
        "Elixir",
        "Tool",
        "Instrument",
        "Symbol",
        "Key/Lock",
        "Gem"
    };
        private static List<string> ItemOrigins = new List<string>
    {
        "Aberration",
        "Elemental",
        "Draconic",
        "Human",
        "Elf/Fey",
        "Dwarf",
        "Orc",
        "Goblinoid",
        "Devil",
        "Demon",
        "Celestial",
        "Undead",
        "Giant",
        "Construct",
    };
        private static List<string> ItemFunction = new List<string>
    {
        "Bane",
        "Worship",
        "Protection",
        "Healing",
        "Summoning",
        "Travel",
        "Communication",
        "Transformation",
        "Containment",
        "Entertainment",
        "Divinination",
        "Augmentation",
        "Creation",
        "Destruction",
        "Chaos",
    };
        private static List<string> ItemConditionDescriptors = new List<string>
    {
        // Pristine
    "Flawless and untouched, as though freshly created",
    "In perfect condition with no visible signs of wear",
    "Immaculate, unmarred by time or use",

    // Polished
    "Carefully polished to a reflective sheen",
    "Smooth and gleaming from regular maintenance",
    "Its surface shines with deliberate care",

    // New
    "Recently made and scarcely used",
    "Still bearing the marks of recent craftsmanship",
    "Freshly acquired and unused",

    // Weathered
    "Worn by years of exposure to the elements",
    "Its surface tells a story of long use and age",
    "Faded and roughened by time",

    // Cracked
    "Fractures run along its surface",
    "Split in places, threatening to break further",
    "Cracked from stress or impact",

    // Rusted
    "Corrosion eats away at its metal parts",
    "Rust flakes off with every movement",
    "Heavily oxidized and weakened",

    // Burned
    "Scorched and blackened by intense heat",
    "Charred marks mar its surface",
    "Warped and burned from fire damage",

    // Water damaged
    "Swollen and softened from prolonged moisture",
    "Water stains and warping are clearly visible",
    "Damaged by soaking and damp conditions",

    // Dirty
    "Caked with grime and filth",
    "Covered in dirt from neglect or travel",
    "Soiled and unpleasant to handle",

    // Blood-stained
    "Spattered with dried blood",
    "Dark stains hint at violent use",
    "Still marked with the remnants of battle",

    // Repaired or reinforced
    "Mended with visible repairs and reinforcements",
    "Strengthened by added bindings and patches",
    "Repaired enough to remain functional",

    // Shattered / Destroyed
    "Broken beyond any practical use",
    "Reduced to fragments and debris",
    "Completely destroyed and unusable",

    // Wrapped / Sealed
    "Carefully wrapped to protect its contents",
    "Sealed to prevent tampering",
    "Bound and enclosed with deliberate care",

    // Organic / Living
    "Warm to the touch and faintly alive",
    "Pulses subtly as though breathing",
    "Organic in nature, with signs of living growth"
    };
        private static List<string> ItemMagicEffectDescriptors = new List<string>
    {
        // Humming
    "It emits a low, steady hum",
    "A faint vibration accompanies a soft humming sound",
    "The air around it resonates quietly",

    // Glow
    "It glows with a steady inner light",
    "A faint radiance spills from its surface",
    "Soft light pulses gently from within",

    // Warm / Cold
    "It feels unnaturally warm to the touch",
    "A lingering chill radiates from it",
    "Its temperature shifts in subtle waves",

    // Calmness
    "A soothing presence eases nearby tension",
    "It instills a sense of calm and clarity",
    "Those nearby feel strangely at peace",

    // Dread
    "An oppressive sense of dread surrounds it",
    "Unease creeps in when standing near it",
    "Its presence fills the air with silent menace",

    // Whispers
    "Faint whispers echo just beyond hearing",
    "Voices murmur softly from nowhere in particular",
    "It seems to speak in hushed, unintelligible tones",

    // Wind
    "A gentle breeze swirls around it",
    "Air currents bend unnaturally in its presence",
    "Wind stirs even in still surroundings",

    // Shadows
    "Shadows cling unnaturally to its form",
    "Nearby shadows twist and stretch",
    "Light seems to dim around it",

    // Reflections
    "Its surface reflects images that seem slightly wrong",
    "Reflections linger longer than they should",
    "Mirrored surfaces nearby behave strangely",

    // Flora wilts
    "Nearby plants wither in its presence",
    "Leaves curl and darken around it",
    "Vegetation slowly decays when it is close",

    // Flora blooms
    "Plants nearby grow rapidly and bloom",
    "Flowers open and turn toward it",
    "Vegetation flourishes in its presence",

    // Other flora reactions
    "Plants react unpredictably when it is near",
    "Vines twist and lean toward it",
    "Moss and roots subtly shift in response",

    // Static
    "The air crackles with faint static",
    "Hair rises slightly from electrical charge",
    "Sparks occasionally jump from its surface",

    // Energy
    "Raw energy radiates outward",
    "It thrums with barely contained power",
    "A surge of magical force surrounds it",

    // Appearance changes
    "Its appearance subtly shifts when observed",
    "Details change when not looked at directly",
    "It never looks quite the same twice"
    };
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            //Debug.WriteLine($"Changed property of Item: {Name}.\nProperty changed: {name}");
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }
        public enum ItemRank
        {
            Common, Uncommon, Rare, VeryRare, Legendary
        }
        private ItemRank _rank;
        public ItemRank Rank
        {
            get => _rank;
            set
            {
                if (_rank != value)
                {
                    _rank = value;
                    OnPropertyChanged(nameof(Rank));
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public enum ItemType
        {
            Minor,
            Major
        }
        private ItemType _type;
        public ItemType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                    OnPropertyChanged(nameof(DisplayName));
                }
            }
        }

        public string Cost { get; set; }
        public bool RequiresAttunement { get; set; }

        private FlowDocument _function = new FlowDocument();
        [JsonConverter(typeof(FlowDocumentJsonConverter))] 
        public FlowDocument Function
        {
            get => _function;
            set => SetProperty(ref _function, value);
        }

        private FlowDocument _appearance = new FlowDocument();
        [JsonConverter(typeof(FlowDocumentJsonConverter))] 
        public FlowDocument Appearance
        {
            get => _appearance;
            set => SetProperty(ref _appearance, value);
        }

        private FlowDocument _origin = new FlowDocument();
        [JsonConverter(typeof(FlowDocumentJsonConverter))] 
        public FlowDocument Origin
        {
            get => _origin;
            set => SetProperty(ref _origin, value);
        }
        [JsonIgnore] public string DisplayName => $"{Name}{RankSuffix}";
        [JsonIgnore] public string RankSuffix => Rank switch
        {
            ItemRank.Common => "",
            ItemRank.Uncommon => " (UC)",
            ItemRank.Rare => " (R)",
            ItemRank.VeryRare => " (VR)",
            ItemRank.Legendary => " (L)",
            _ => ""
        };

        [JsonIgnore, ObservableProperty] public bool isNew = false;


        [JsonIgnore, ObservableProperty] public Visibility expandedVisibility = Visibility.Collapsed;

        public Item()
        {
            
        }
        public Item(Item itemToCopy)
        {
            Name = "Copy of " + itemToCopy.Name;
            Rank = itemToCopy.Rank;
            Type = itemToCopy.Type;
            RequiresAttunement = itemToCopy.RequiresAttunement;
            Function = itemToCopy.Function;
            Appearance = itemToCopy.Appearance;
            Origin = itemToCopy.Origin;
            IsNew = true;
        }

        public static string GetRandomName()
        {
            Random _random = new Random();
            string row = ItemTypes[_random.Next(ItemTypes.Count)];
            return row + " of " + NPC.GetRandomName(_random.Next(2) == 0 ? "Male" : "Female");
        }

        public static string GetRandomItemOrigin()
        {
            Random _random = new Random();
            string row = ItemOrigins[_random.Next(ItemOrigins.Count)];
            return row;
        }

        public static string GetRandomFunction()
        {
            Random _random = new Random();
            string function = ItemFunction[_random.Next(ItemFunction.Count)];
            return function;
        }
        public static string GetRandomAppearance()
        {
            Random _random = new Random();
            string condition = ItemConditionDescriptors[_random.Next(ItemConditionDescriptors.Count)];
            string magicEffect = ItemMagicEffectDescriptors[_random.Next(ItemMagicEffectDescriptors.Count)];
            return condition + "\n" + magicEffect;
        }
    }
}
