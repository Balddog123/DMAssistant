using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DMAssistant.Services
{
    public interface IAudioPlayerService
    {
        void Play(string path);
        void Pause();
        void Stop();
        void Resume();
        void Seek(double seconds);
        void SetVolume(double volume);
        double GetPositionSeconds();
        void ToggleLoop();
        bool isLooping { get; }
    }

    public class AudioPlayerService : IAudioPlayerService
    {
        private readonly MediaElement _player;

        public AudioPlayerService(MediaElement player)
        {
            _player = player;
            _player.MediaEnded += (s, e) => OnEnded?.Invoke();
        }

        public event Action? OnEnded;
        public bool isLooping { get; private set; }

        public void Play(string path)
        {
            _player.Source = new Uri(path);
            _player.Play();
        }

        public void Pause() => _player.Pause();
        public void Stop() => _player.Stop();
        public void Resume() => _player.Play();
        public void SetVolume(double v) => _player.Volume = v;
        public void Seek(double seconds) => _player.Position = TimeSpan.FromSeconds(seconds);

        public double GetPositionSeconds() => _player.Position.TotalSeconds;
        public void ToggleLoop()
        {
            isLooping = !isLooping;
        }
    }


}
