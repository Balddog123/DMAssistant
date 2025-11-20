using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using DMAssistant.Model;

namespace DMAssistant.ViewModel
{
    public class SessionViewModel : ObservableObject
    {
        public ObservableCollection<Session> Sessions
            => App.CampaignStore.CurrentCampaign.Sessions;

        private Session? _selectedSession;
        public Session? SelectedSession
        {
            get => _selectedSession;
            set
            {
                if (SetProperty(ref _selectedSession, value))
                {
                    // When a session is selected, load its NPCs
                    if (_selectedSession != null)
                    {
                        NPCPanel = new NPCPanelViewModel(_selectedSession.NPCIDs, _selectedSession);
                        ItemPanel = new ItemPanelViewModel(_selectedSession.ItemIDs, _selectedSession);
                        EncountersPanel = new EncountersPanelViewModel(_selectedSession);
                        LocationPanel = new LocationPanelViewModel(_selectedSession.LocationIDs, _selectedSession);
                    }
                    else
                    {
                        NPCPanel = null;
                        ItemPanel = null;
                        EncountersPanel = null;
                        LocationPanel = null;
                    }

                    // Notify UI that scene/secret/notes values changed
                    OnPropertyChanged(nameof(SceneText));
                    OnPropertyChanged(nameof(Secrets));
                    OnPropertyChanged(nameof(NotesText));
                }
            }
        }

        private NPCPanelViewModel? _npcPanel;
        public NPCPanelViewModel? NPCPanel
        {
            get => _npcPanel;
            set => SetProperty(ref _npcPanel, value);
        }

        private ItemPanelViewModel? _itemPanel;
        public ItemPanelViewModel? ItemPanel
        {
            get => _itemPanel;
            set => SetProperty(ref _itemPanel, value);
        }
        private EncountersPanelViewModel? _encountersPanel;
        public EncountersPanelViewModel? EncountersPanel
        {
            get => _encountersPanel;
            set => SetProperty(ref _encountersPanel, value);
        }
        private LocationPanelViewModel? _locationPanel;
        public LocationPanelViewModel? LocationPanel
        {
            get => _locationPanel;
            set => SetProperty(ref _locationPanel, value);
        }
        public string SceneText
        {
            get => _selectedSession?.Scenes ?? "";
            set
            {
                if (_selectedSession != null && _selectedSession.Scenes != value)
                {
                    _selectedSession.Scenes = value;
                    OnPropertyChanged(); // Notify the UI
                }
            }
        }

        //SECRETS
        public ObservableCollection<ChecklistItem> Secrets
        {
            get => _selectedSession?.Secrets ?? new ObservableCollection<ChecklistItem>();
            set
            {
                if (_selectedSession != null && _selectedSession.Secrets != value)
                {
                    _selectedSession.Secrets = value;
                    OnPropertyChanged(); // Notify the UI
                }
            }
        }
        public string NewSecretText { get; set; }

        public ICommand AddSecretCommand => new RelayCommand(() =>
        {
            if (_selectedSession == null) return;

            _selectedSession.Secrets.Add(new ChecklistItem());
            OnPropertyChanged(nameof(Secrets));
        });

        public ICommand RemoveSecretCommand => new RelayCommand<ChecklistItem>(item =>
        {
            if (_selectedSession == null || item == null) return;

            _selectedSession.Secrets.Remove(item);
            OnPropertyChanged(nameof(Secrets));
        });

        public string NotesText
        {
            get => _selectedSession?.Notes ?? "";
            set
            {
                if (_selectedSession != null && _selectedSession.Notes != value)
                {
                    _selectedSession.Notes = value;
                    OnPropertyChanged();
                }
            }
        }


        public ICommand AddSessionCommand { get; }

        public SessionViewModel()
        {
            AddSessionCommand = new RelayCommand(() =>
            {
                var newSession = new Session { Name = $"Session {Sessions.Count + 1}" };
                Sessions.Add(newSession);

                // Automatically select it
                SelectedSession = newSession;

                Debug.WriteLine("New session added!");
            });
        }
    }
}
