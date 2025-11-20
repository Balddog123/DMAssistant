using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.View;
using DMAssistant.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DMAssistant.ViewModel
{
    public class EncountersPanelViewModel : ObservableObject
    {
        private Session session;
        public ObservableCollection<Encounter> Encounters { get; }

        private EncounterViewModel _selectedEncounterViewModel;
        public EncounterViewModel SelectedEncounterViewModel
        {
            get => _selectedEncounterViewModel;
            set => SetProperty(ref _selectedEncounterViewModel, value);
        }
        private Encounter _selectedEncounter;
        public Encounter SelectedEncounter {
            get => _selectedEncounter;
            set
            {
                if (SetProperty(ref _selectedEncounter, value)) SelectedEncounterViewModel = new EncounterViewModel(_selectedEncounter);
            }
        }

        public ICommand AddEncounterCommand { get; }
        public ICommand RemoveEncounterCommand { get; }

        public EncountersPanelViewModel(Session session)
        {
            AddEncounterCommand = new RelayCommand(AddEncounter);
            RemoveEncounterCommand = new RelayCommand<Encounter>(RemoveEncounter);
            this.session = session;

            Encounters = session.Encounters;
        }

        private void AddEncounter()
        {
            Encounter newEncounter = new Encounter();
            var encounterVM = new EncounterViewModel(newEncounter);

            Encounters.Add(newEncounter);
            SelectedEncounter = newEncounter;
        }

        private void RemoveEncounter(Encounter? encounter)
        {
            if(encounter != null)
            {
                Encounters.Remove(encounter);
            }
        }
    }
}
