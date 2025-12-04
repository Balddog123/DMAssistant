using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.ViewModel
{
    public class SoundPanelViewModel : ObservableObject
    {
        public SoundViewModel MusicSoundView { get; set; }
        public SoundViewModel AmbienceSoundView { get; set; }
        public SoundViewModel SoundboardSoundView { get; set; }
        public SoundPanelViewModel()
        {
            MusicSoundView = new SoundViewModel(MainWindow.MusicPlayer);
            MainWindow.MusicPlayer.Player.SetVolume(App.AudioStore.AudioSettings.musicVolume);
            AmbienceSoundView = new SoundViewModel(MainWindow.AmbiencePlayer);
            MainWindow.AmbiencePlayer.Player.SetVolume(App.AudioStore.AudioSettings.ambienceVolume);
            SoundboardSoundView = new SoundViewModel(MainWindow.SoundPlayer);
            MainWindow.SoundPlayer.Player.SetVolume(App.AudioStore.AudioSettings.soundVolume);
        }
    }
}
