using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Helpers;
using DMAssistant.Model;
using DMAssistant.Services;
using DMAssistant.Store;
using DMAssistant.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DMAssistant.ViewModel
{
    public partial class SoundViewModel : ObservableObject
    {
        private static string ambiencePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Ambience");

        public ObservableCollection<AudioFile> AudioFiles { get; set; } = new();
        public ObservableCollection<Playlist> Playlists
        {
            get => App.AudioStore.AudioSettings.Playlists;
            set
            {
                App.AudioStore.AudioSettings.Playlists = value;
                OnPropertyChanged();
            }
        }

        private AudioFile _selectedAudio;
        public AudioFile SelectedAudio
        {
            get => _selectedAudio;
            set { _selectedAudio = value; OnPropertyChanged(); }
        }

        private AudioQueueElement _playingAudio;
        public AudioQueueElement PlayingAudio
        {
            get => _playingAudio;
            set { _playingAudio = value; OnPropertyChanged(); }
        }

        private Playlist _selectedPlaylist;
        public Playlist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set { _selectedPlaylist = value; OnPropertyChanged(); }
        }
        private bool _isPlaylistPopupOpen = false;
        public bool IsPlaylistPopupOpen
        {
            get => _isPlaylistPopupOpen;
            set => SetProperty(ref _isPlaylistPopupOpen, value);
        }

        //player controls
        [ObservableProperty] private double duration;
        partial void OnDurationChanged(double oldValue, double newValue)
        {
            OnPropertyChanged(nameof(PositionDisplay));
        }
        [ObservableProperty] private double position;
        partial void OnPositionChanged(double oldValue, double newValue)
        {
            OnPropertyChanged(nameof(PositionDisplay));
        }
        [ObservableProperty] private double volume;
        partial void OnVolumeChanged(double value)
        {
            AudioPlayerService.SetVolume(value);
        }
        public string PositionDisplay => $"{TimeSpan.FromSeconds(Position):m\\:ss} / {TimeSpan.FromSeconds(Duration):m\\:ss}";
        public IRelayCommand PlayCommand { get; }
        public IRelayCommand PauseCommand { get; }
        public IRelayCommand StopCommand { get; }
        public IRelayCommand LoopCommand { get; }
        public RelayCommand PlaySelectedAudio { get; }

        private Thickness _loopThickness = new Thickness(0);
        public Thickness LoopThickness { get => _loopThickness; set => SetProperty( ref _loopThickness, value ); }

        public readonly IAudioPlayerService AudioPlayerService;

        //menus
        [ObservableProperty] private Visibility libraryVisibility = Visibility.Visible;
        [ObservableProperty] private Visibility playlistsVisibility = Visibility.Collapsed;
        [ObservableProperty] private Visibility queueVisibility = Visibility.Collapsed;
        public RelayCommand ShowLibraryCommand { get; }
        public RelayCommand ShowPlaylistsCommand { get; }
        public RelayCommand ShowQueueCommand { get; }

        //library        
        public ICommand LoadDirectoryCommand { get; }
        public ICommand FindDirectoryCommand { get; }

        //playlists
        public ICommand ShowPlaylistPopupCommand { get; }
        public ICommand AddToPlaylistCommand { get; }
        public ICommand CreatePlaylistCommand { get; }
        public ICommand PlaySelectedPlaylistCommand { get; }
        public ICommand RemoveFromSelectedPlaylistCommand { get; }
        public ICommand SelectPlaylistCommand { get; }
        public ICommand CloseSelectedPlaylistCommand { get; }
        private UIElement _popupPlacementTarget;
        public UIElement PopupPlacementTarget
        {
            get => _popupPlacementTarget;
            set => SetProperty(ref _popupPlacementTarget, value);
        }
        [ObservableProperty] private Visibility selectedPlaylistVisibility = Visibility.Collapsed;


        //queue
        private ObservableCollection<AudioQueueElement> _queueView = new();
        public ObservableCollection<AudioQueueElement> AudioQueue
        {
            get => _queueView;
            private set => SetProperty(ref _queueView, value);
        }
        public ICommand AddToQueue { get; }
        public ICommand RemoveFromQueueCommand { get; }
        public ICommand ClearQueueCommand { get; }

        public SoundViewModel(IAudioPlayerService audioService)
        {
            AudioPlayerService = audioService;
            LibraryVisibility = Visibility.Visible;
            PlaylistsVisibility = Visibility.Collapsed;
            QueueVisibility = Visibility.Collapsed;
            SelectedPlaylistVisibility = Visibility.Collapsed;

            //controls
            PlayCommand = new RelayCommand(PressPlay);
            PlaySelectedAudio = new RelayCommand(() => AudioPlayerService.Play(SelectedAudio?.FilePath));
            PauseCommand = new RelayCommand(() => AudioPlayerService.Pause());
            StopCommand = new RelayCommand(() =>
            {
                AudioPlayerService.Stop();
                PlayingAudio = null;
                if(AudioQueue.Count > 0) AudioQueue.RemoveAt(0);
                Duration = 0.0;
                Position = 0.0;
            });
            LoopCommand = new RelayCommand(() =>
            {
                AudioPlayerService.ToggleLoop();
                LoopThickness = _loopThickness.Left == 0 ? new Thickness(5) : new Thickness(0);
            });
            volume = 1.0;

            //library
            LoadDirectoryCommand = new RelayCommand(LoadAudioFiles);
            FindDirectoryCommand = new RelayCommand(FindDirectory);
            ShowLibraryCommand = new RelayCommand(() =>
            {
                LibraryVisibility = Visibility.Visible;
                PlaylistsVisibility = Visibility.Collapsed;
                QueueVisibility = Visibility.Collapsed;
            });

            //playlist
            ShowPlaylistPopupCommand = new RelayCommand<UIElement>(element =>
            {
                if (element is FrameworkElement fe && fe.DataContext is AudioFile audioFile)
                {
                    SelectedAudio = audioFile;
                }
                PopupPlacementTarget = element;
                IsPlaylistPopupOpen = true;
            });
            ShowPlaylistsCommand = new RelayCommand(() =>
            {
                LibraryVisibility = Visibility.Collapsed;
                PlaylistsVisibility = Visibility.Visible;
                QueueVisibility = Visibility.Collapsed;
            });
            AddToPlaylistCommand = new RelayCommand<Playlist>(playlist =>
            {
                if (playlist != null && SelectedAudio != null)
                {
                    playlist.Files.Add(SelectedAudio);
                    foreach (AudioFile file in playlist.Files)
                    {
                        Debug.WriteLine($"{playlist.Name}: {file.FileName}");
                    }
                    IsPlaylistPopupOpen = false;
                }
            });
            RemoveFromSelectedPlaylistCommand = new RelayCommand<AudioFile>(audioToRemove => RemoveFromSelectedPlaylist(audioToRemove));
            SelectPlaylistCommand = new RelayCommand<Playlist>((playlist) =>
            {
                Debug.WriteLine("Selected playlist " + playlist.Name);
                SelectedPlaylistVisibility = Visibility.Visible;
                SelectedPlaylist = playlist;
            });
            CloseSelectedPlaylistCommand = new RelayCommand(() =>
            {
                SelectedPlaylistVisibility = Visibility.Collapsed;
                SelectedPlaylist = null;
            });
            CreatePlaylistCommand = new RelayCommand(CreatePlaylist);
            PlaySelectedPlaylistCommand = new RelayCommand(PlaySelectedPlaylist);

            //queue
            AddToQueue = new RelayCommand<AudioFile>(audio =>
            {
                AudioQueue.Add(new AudioQueueElement(audio));
                if (AudioQueue.Count == 1) Play(audio);
            });
            RemoveFromQueueCommand = new RelayCommand<string>(queueId =>
            {
                bool playNext = false;
                if (AudioQueue[0].Id == queueId)
                {
                    AudioPlayerService.Stop();
                    playNext = true;
                }
                // Rebuild the queue without the item to remove
                AudioQueue = new ObservableCollection<AudioQueueElement>(AudioQueue.Where(q => q.Id != queueId));
                if (playNext && AudioQueue.Count > 0) Play(AudioQueue[0].File);
                else if(AudioQueue.Count == 0)
                {
                    PlayingAudio = null;
                }

            });
            ShowQueueCommand = new RelayCommand(() =>
            {
                LibraryVisibility = Visibility.Collapsed;
                PlaylistsVisibility = Visibility.Collapsed;
                QueueVisibility = Visibility.Visible;
            });
            ClearQueueCommand = new RelayCommand(ClearQueue);

            LoadAudioFiles();
        }

        private void FindDirectory()
        {
            OpenFolderDialog dialog = new OpenFolderDialog()
            {
                InitialDirectory = App.AudioStore.AudioSettings.MusicPath,
                Title = "Set Music Folder"
            };

            // Show dialog
            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                
                string selectedFolder = dialog.FolderName;
                Debug.WriteLine(selectedFolder);
                App.AudioStore.AudioSettings.MusicPath = selectedFolder;
                LoadAudioFiles();
            }
        }

        public void HandleAudioEnded()
        {
            Debug.WriteLine("Handling audio ending at view model...");
            AudioQueue.RemoveAt(0);

            if (AudioQueue.Count > 0) Play(AudioQueue[0].File);
            else
            {
                PlayingAudio = null;
            }

        }

        private void ClearQueue()
        {
            AudioQueue.Clear();
            AudioPlayerService.Stop();
            PlayingAudio = null;
        }

        private void LoadAudioFiles()
        {
            AudioFiles.Clear();
            if (!Directory.Exists(App.AudioStore.AudioSettings.MusicPath)) 
            {
                Debug.WriteLine("MusicPath doesn't exist, sorry bro...");
                return; 
            } //possibly create directories and move this check to the beginning of the app initialization

            var path = App.AudioStore.AudioSettings.MusicPath;

            var files = Directory.GetFiles(path, "*.mp3")
                .Concat(Directory.GetFiles(path, "*.wav"));

            foreach (var file in files)
                AudioFiles.Add(new AudioFile { FilePath = file });

        }

        private void AddToPlaylist()
        {
            if (SelectedAudio != null && SelectedPlaylist != null && !SelectedPlaylist.Files.Contains(SelectedAudio))
                SelectedPlaylist.Files.Add(SelectedAudio);
        }
        private void RemoveFromSelectedPlaylist(AudioFile audioToRemove)
        {
            if(audioToRemove != null && SelectedPlaylist != null && SelectedPlaylist.Files.Contains(SelectedAudio))
            {
                SelectedPlaylist.Files.Remove(audioToRemove);
            }
        }

        private void CreatePlaylist()
        {
            Playlists.Add(new Playlist() { Name = $"Playlist {Playlists.Count + 1}" });
        }

        private void PlaySelectedPlaylist()
        {
            if (SelectedPlaylist != null && SelectedPlaylist.Files.Count > 0)
            {
                AudioQueue.Clear();
                foreach(var file in SelectedPlaylist.Files)
                {
                    AudioQueue.Add(new AudioQueueElement(file));
                }
                Play(AudioQueue[0].File);
            }
        }

        private void PressPlay()
        {
            if(PlayingAudio != null)
            {
                Resume();
            }
            else
            {
                Play(SelectedAudio);
            }
        }

        public void Play(AudioFile playingAudio)
        {
            PlayingAudio = new AudioQueueElement(playingAudio);
            if(PlayingAudio.File != null) AudioPlayerService.Play(PlayingAudio.File.FilePath);
        }

        private void Resume()
        {
            AudioPlayerService.Resume();
        }
    }
}
