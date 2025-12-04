using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public partial class AudioSettings : ObservableObject
    {
        private ObservableCollection<Playlist> _musicPlaylists = new();
        public ObservableCollection<Playlist> MusicPlaylists
        {
            get => _musicPlaylists;
            set
            {
                _musicPlaylists = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Playlist> _ambiencePlaylists = new();
        public ObservableCollection<Playlist> AmbiencePlaylists
        {
            get => _ambiencePlaylists;
            set
            {
                _ambiencePlaylists = value;
                OnPropertyChanged();
            }
        }

        [ObservableProperty] public string musicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Music");
        [ObservableProperty] public string ambiencePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Ambience");
    }
}
