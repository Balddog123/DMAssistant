using DMAssistant.Model;
using DMAssistant.Services;
using DMAssistant.ViewModel;
using DMAssistant.Helpers;
using MahApps.Metro.Controls;
using System.Diagnostics;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DMAssistant.View
{
    public partial class MainWindow : MetroWindow
    {
        public static AudioController MusicPlayer { get; private set; }
        public static AudioController AmbiencePlayer { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            MusicPlayer = new AudioController(GlobalMediaPlayer);
            MusicPlayer.name = "music";
            AmbiencePlayer = new AudioController(GlobalAmbiencePlayer);
            AmbiencePlayer.name = "ambience";

            DataContext = new MainWindowViewModel();
        }

        public void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            if ((MediaElement)sender == GlobalMediaPlayer) MusicPlayer.OnMediaOpenedHandler(sender, e);
            if ((MediaElement)sender == GlobalAmbiencePlayer) AmbiencePlayer.OnMediaOpenedHandler(sender, e);
        }

        public void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if ((MediaElement)sender == GlobalMediaPlayer) MusicPlayer.OnMediaEndedHandler(sender, e);
            if ((MediaElement)sender == GlobalAmbiencePlayer) AmbiencePlayer.OnMediaEndedHandler(sender, e);
        }
    }
}

