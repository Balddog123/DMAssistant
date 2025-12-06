using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [ObservableProperty] public string description;
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
        //public Spell(CastingTime castingTime, ObservableCollection<string> classes, Components components, string description, string duration, string level, string name, string range, bool ritual, SchoolOfMagic school)
        //{
        //    this.castingTime = castingTime;
        //    Classes = classes;
        //    this.components = components;
        //    this.description = description;
        //    this.duration = duration;
        //    this.level = level;
        //    this.name = name;
        //    this.range = range;
        //    this.ritual = ritual;
        //    this.school = school;
        //}
    }

    public partial class Components : ObservableObject
    {
        [ObservableProperty] public bool material;
        [ObservableProperty] public List<string> raw;
        [ObservableProperty] public bool somatic;
        [ObservableProperty] public bool verbal;
    }
}
