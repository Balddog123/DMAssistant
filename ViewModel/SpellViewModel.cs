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
        private Spell spell;
        public Spell Spell
        {
            get => spell; 
            set
            {
                SetProperty(ref spell, value);
            }
        }


        public string Name
        {
            get => Spell != null ? Spell.Name : string.Empty;
            set
            {
                if (IsNew && value != "New Spell") IsNew = false;
                SetProperty(Spell.Name, value, Spell, (m, v) => m.Name = v);
                OnPropertyChanged();
            }
        }
        public string Level
        {
            get => Spell != null ? Spell.Level : string.Empty;
            set
            {
                SetProperty(Spell.Level, value, Spell, (m, v) => m.Level = v);
                OnPropertyChanged();
            }
        }

        private bool _isNew;
        public bool IsNew
        {
            get => _isNew;
            set => SetProperty(ref _isNew, value);
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
        }

        public SpellViewModel(Spell spell)
        {
            Spell = spell;
        }
    }
}