using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DMAssistant.ViewModel
{
    public class SpellViewModel : ObservableObject
    {
        // The Spell item being edited
        private Spell spell;
        public Spell Spell
        {
            get => spell; set
            {
                SetProperty(ref spell, value);
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Level));
            }
        }


        public string Name
        {
            get => Spell.Name;
        }
        public string Level
        {
            get => Spell.Level;
        }


        // ComboBox: Casting Time
        public Array CastingTimes => Enum.GetValues(typeof(CastingTime));

        // ComboBox: School of Magic
        public Array SchoolsOfMagic => Enum.GetValues(typeof(Spell.SchoolOfMagic));

        // Editable list of spell classes (wizard, cleric, sorcerer, etc.)
        public ObservableCollection<string> AvailableClasses { get; } =
            new ObservableCollection<string>
            {
                "bard", "cleric", "druid",
                "paladin", "ranger", "sorcerer",
                "warlock", "wizard", "artificer"
            };

        public SpellViewModel()
        {
            // Default empty spell
            Spell = new Spell();
            //Spell = new Spell(
            //    castingTime: CastingTime.Action,
            //    classes: new ObservableCollection<string>(),
            //    components: new Components
            //    {
            //        material = false,
            //        somatic = false,
            //        verbal = false,
            //        raw = new List<string>()
            //    },
            //    description: "",
            //    duration: "",
            //    level: "",
            //    name: "",
            //    range: "",
            //    ritual: false,
            //    school: Spell.SchoolOfMagic.Conjuration
            //);
        }

        public SpellViewModel(Spell spell)
        {
            Spell = spell;
        }
    }
}