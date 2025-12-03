using DMAssistant.Model;
using DMAssistant.Services;
using DMAssistant.ViewModel;
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
        public static IAudioPlayerService AudioPlayer { get; private set; }
        public static DispatcherTimer AudioTimer { get; private set; }
        public static Action<double>? OnMediaOpened;
        public static Action OnMediaEnded;

        public MainWindow()
        {
            InitializeComponent();
            AudioPlayer = new AudioPlayerService(GlobalMediaPlayer);

            DataContext = new MainWindowViewModel();

            AudioTimer = new DispatcherTimer();
            AudioTimer.Interval = TimeSpan.FromMilliseconds(200);
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (GlobalMediaPlayer.NaturalDuration.HasTimeSpan)
            {
                OnMediaOpened?.Invoke(GlobalMediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
            }

            AudioTimer.Start();
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Media ended");
            if (AudioPlayer.isLooping)
            {
                GlobalMediaPlayer.Position = TimeSpan.FromSeconds(0);
            }
            else
            {

                GlobalMediaPlayer.Stop();
                OnMediaEnded?.Invoke();
            }
        }
    }
}

