using DMAssistant.Services;
using DMAssistant.View;
using DMAssistant.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DMAssistant.Helpers
{
    public class AudioController
    {
        public SoundViewModel.SoundViewType soundViewType;
        public IAudioPlayerService Player { get; }
        public DispatcherTimer Timer { get; }

        public event Action<double>? MediaOpened;
        public event Action? MediaEnded;

        private readonly MediaElement _mediaElement;

        public AudioController(MediaElement mediaElement)
        {
            _mediaElement = mediaElement;

            Player = new AudioPlayerService(mediaElement);

            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };

            //_mediaElement.MediaOpened += OnMediaOpenedHandler;
            //_mediaElement.MediaEnded += OnMediaEndedHandler;
        }

        public void OnMediaOpenedHandler(object sender, RoutedEventArgs e)
        {
            if (_mediaElement.NaturalDuration.HasTimeSpan)
            {
                MediaOpened?.Invoke(_mediaElement.NaturalDuration.TimeSpan.TotalSeconds);
            }
            Timer.Start();
        }

        public void OnMediaEndedHandler(object sender, RoutedEventArgs e)
        {
            if (Player.LoopMode == LoopMode.Single)
            {
                _mediaElement.Position = TimeSpan.Zero;
                return;
            }

            _mediaElement.Stop();
            MediaEnded?.Invoke();
        }
    }

}
