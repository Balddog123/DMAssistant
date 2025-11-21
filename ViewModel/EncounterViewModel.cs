using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace DMAssistant.ViewModel
{
    public class EncounterViewModel : ObservableObject
    {
        private Encounter _encounter;
        public Encounter Encounter 
        { 
            get => _encounter != null ? _encounter : new();
            set
            {
                SetProperty(ref _encounter, value);
            }
        }
        public string Name
        {
            get => Encounter != null ? Encounter.Name : string.Empty;
            set => SetProperty(Encounter.Name, value, Encounter, (e, v) => e.Name = v);
        }

        
        public ObservableCollection<EncounterItem> EncounterItems
        {
            get => Encounter != null ? Encounter.EncounterItems : new();
            set => SetProperty(Encounter.EncounterItems, value, Encounter, (e, v) => e.EncounterItems = v);
        }

        public ICommand AddMonsterCommand { get; }
        public ICommand AddEventCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand RunEncounterCommand { get; }

        private EncounterItem _selectedItem;
        public EncounterItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                    SelectedEncounterItemViewModel = new EncounterItemViewModel(_selectedItem);
            }
        }
        private EncounterItemViewModel _selectedEncounterItemViewModel;
        public EncounterItemViewModel SelectedEncounterItemViewModel
        {
            get => _selectedEncounterItemViewModel;
            set => SetProperty(ref _selectedEncounterItemViewModel, value);
        }
        private CombatTrackerViewModel _currentCombat;
        public CombatTrackerViewModel CurrentCombat
        {
            get => _currentCombat;
            set => SetProperty(ref _currentCombat, value);
        }


        public EncounterViewModel(Encounter encounter)
        {
            Encounter = encounter;

            AddMonsterCommand = new RelayCommand(AddNewMonster);
            AddEventCommand = new RelayCommand(AddNewEvent);
            RemoveItemCommand = new RelayCommand<EncounterItem>(RemoveEncounterItem);
            RunEncounterCommand = new RelayCommand(RunEncounter);

            if(encounter.CombatItems.Count > 0)
            {
                CombatTrackerViewModel newCombat = new CombatTrackerViewModel(Encounter, Encounter.CombatItems);
                CurrentCombat = newCombat;
            }
        }

        private void RunEncounter()
        {
            Encounter.CurrentRound = 0;
            CombatTrackerViewModel newCombat = new CombatTrackerViewModel(Encounter, null);
            CurrentCombat = newCombat;
        }

        void AddNewMonster()
        {
            MonsterGroup newMonsterGroup = new MonsterGroup();
            EncounterItems.Add(newMonsterGroup);
        }
        void AddNewEvent()
        {
            EncounterEvent newEvent = new EncounterEvent();
            EncounterItems.Add(newEvent);
        }
        void RemoveEncounterItem(EncounterItem? item)
        {
            if(item != null)
            {
                EncounterItems.Remove(item);
            }
        }
    }
}
