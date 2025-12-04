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
        public SoundPanelViewModel()
        {
            MusicSoundView = new SoundViewModel(MainWindow.MusicPlayer);
            AmbienceSoundView = new SoundViewModel(MainWindow.AmbiencePlayer);
        }
    }
}
