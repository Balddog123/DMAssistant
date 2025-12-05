using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DMAssistant.Services
{
    public enum LoopMode
    {
        None,
        Single,
        All
    }
    public interface IAudioPlayerService
    {
        void Play(string path);
        void Pause();
        void Stop();
        void Resume();
        void Seek(double seconds);
        void SetVolume(double volume);
        double GetVolume { get; }
        double GetPositionSeconds();
        LoopMode LoopMode { get; set; }
        bool IsShuffling { get; set; }

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
        public bool IsShuffling { get; set; }
        public LoopMode LoopMode { get; set; }

        public void Play(string path)
        {
            _player.Source = new Uri(path);
            _player.Play();
        }

        public void Pause() => _player.Pause();
        public void Stop() => _player.Stop();
        public void Resume() => _player.Play();
        public void SetVolume(double v) => _player.Volume = v;
        public double GetVolume
        {
            get
            {
                return _player.Volume;
            }
        }
        public void Seek(double seconds) => _player.Position = TimeSpan.FromSeconds(seconds);

        public double GetPositionSeconds() => _player.Position.TotalSeconds;
    }


}
