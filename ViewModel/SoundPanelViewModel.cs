using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
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
    public partial class SoundPanelViewModel : ObservableObject
    {
        private static string musicPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Music");
        private static string ambiencePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Ambience");

        public ObservableCollection<AudioFile> AudioFiles { get; set; } = new();
        private ObservableCollection<Playlist> _playlists = new();
        public ObservableCollection<Playlist> Playlists
        {
            get => _playlists;
            set
            {
                _playlists = value;
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
            VolumeChanged?.Invoke(value);
        }
        public string PositionDisplay => $"{TimeSpan.FromSeconds(Position):m\\:ss} / {TimeSpan.FromSeconds(Duration):m\\:ss}";
        public IRelayCommand PlayCommand { get; }
        public IRelayCommand PauseCommand { get; }
        public IRelayCommand StopCommand { get; }
        public IRelayCommand LoopCommand { get; }
        public RelayCommand PlaySelectedAudio { get; }
        public Action PauseRequested;
        public Action StopRequested;
        public Action LoopRequested;
        public event Action<string>? PlayAudioRequested;
        public event Action? ResumeRequested;
        public Action<double> VolumeChanged;
        private Thickness _loopThickness = new Thickness(0);
        public Thickness LoopThickness { get => _loopThickness; set => SetProperty( ref _loopThickness, value ); }

        //menus
        [ObservableProperty] private Visibility libraryVisibility = Visibility.Visible;
        [ObservableProperty] private Visibility playlistsVisibility = Visibility.Collapsed;
        [ObservableProperty] private Visibility queueVisibility = Visibility.Collapsed;
        public RelayCommand ShowLibraryCommand { get; }
        public RelayCommand ShowPlaylistsCommand { get; }
        public RelayCommand ShowQueueCommand { get; }

        //library        
        public ICommand LoadDirectoryCommand { get; }

        //playlists
        public ICommand ShowPlaylistPopupCommand { get; }
        public ICommand AddToPlaylistCommand { get; }
        public ICommand CreatePlaylistCommand { get; }
        public ICommand PlayPlaylistCommand { get; }
        private UIElement _popupPlacementTarget;
        public UIElement PopupPlacementTarget
        {
            get => _popupPlacementTarget;
            set => SetProperty(ref _popupPlacementTarget, value);
        }

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

        

        public SoundPanelViewModel()
        {
            //controls
            PlayCommand = new RelayCommand(PressPlay);
            PlaySelectedAudio = new RelayCommand(() => PlayAudioRequested?.Invoke(SelectedAudio?.FilePath));
            PauseCommand = new RelayCommand(() => PauseRequested?.Invoke());
            StopCommand = new RelayCommand(() => StopRequested?.Invoke());
            LoopCommand = new RelayCommand(() =>
            {
                LoopRequested?.Invoke();
                LoopThickness = _loopThickness.Left == 0 ? new Thickness(5) : new Thickness(0);
                Debug.WriteLine(LoopThickness);
            });
            volume = 1.0;

            //library
            LoadDirectoryCommand = new RelayCommand(LoadAudioFiles);
            ShowLibraryCommand = new RelayCommand(() =>
            {
                LibraryVisibility = Visibility.Visible;
                PlaylistsVisibility = Visibility.Collapsed;
                QueueVisibility = Visibility.Collapsed;
            });

            //playlist
            Playlists.Add(new Playlist() { Name = "Awesome Songs", });

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
            CreatePlaylistCommand = new RelayCommand(CreatePlaylist);
            PlayPlaylistCommand = new RelayCommand<Playlist>(playlist => PlayPlaylist(playlist));

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
                    StopRequested?.Invoke();
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

        public void HandleAudioEnded()
        {
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
            StopRequested?.Invoke();
        }

        private void LoadAudioFiles()
        {
            AudioFiles.Clear();
            if (!Directory.Exists(musicPath)) return; //possibly create directories and move this check to the beginning of the app initialization

            foreach (var file in Directory.GetFiles(musicPath, "*.mp3" ))
                AudioFiles.Add(new AudioFile { FilePath = file });
        }

        private void AddToPlaylist()
        {
            if (SelectedAudio != null && SelectedPlaylist != null && !SelectedPlaylist.Files.Contains(SelectedAudio))
                SelectedPlaylist.Files.Add(SelectedAudio);
        }

        private void CreatePlaylist()
        {
            Playlists.Add(new Playlist() { Name = $"Playlist {Playlists.Count + 1}" });
        }

        private void PlayPlaylist(Playlist playlist)
        {
            if (playlist.Files.Count > 0)
            {
                AudioQueue.Clear();
                foreach(var file in playlist.Files)
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
            PlayAudioRequested?.Invoke(PlayingAudio.File.FilePath);
        }

        private void Resume()
        {
            ResumeRequested?.Invoke();
        }
    }
}
