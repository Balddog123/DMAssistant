using DMAssistant.Model;
using DMAssistant.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DMAssistant.View
{
    public partial class SoundView : UserControl
    {
        private bool _userIsDragging = false;
        private bool _isLooping = false;

        public SoundView()
        {
            InitializeComponent();

            MainWindow.MusicPlayer.Timer.Tick += UpdatePosition;
            MainWindow.MusicPlayer.MediaOpened += (seconds) =>
            {
                var vm = (SoundViewModel)DataContext;
                if(vm != null) vm.Duration = seconds;
            };
            MainWindow.MusicPlayer.MediaEnded += () =>
            {
                var vm = (SoundViewModel)DataContext;
                if (vm != null) vm.HandleAudioEnded();
            };

            MainWindow.AmbiencePlayer.Timer.Tick += UpdatePosition;
            MainWindow.AmbiencePlayer.MediaOpened += (seconds) =>
            {
                var vm = (SoundViewModel)DataContext;
                if (vm != null) vm.Duration = seconds;
            };
            MainWindow.AmbiencePlayer.MediaEnded += () =>
            {
                var vm = (SoundViewModel)DataContext;
                if (vm != null) vm.HandleAudioEnded();
            };
        }

        private void AudioList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBox lb && lb.SelectedItem is AudioFile audio)
            {
                var vm = DataContext as SoundViewModel;

                if (vm != null)
                {
                    Debug.WriteLine($"Double clicked! {audio.FilePath}");
                    vm.SelectedAudio = audio;
                    AudioQueueElement element = new AudioQueueElement(audio);
                    vm.Play(element);
                    vm.AudioQueue.Insert(0, element);
                }
            }
        }

        private void PlaylistItemClicked(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SoundViewModel vm)
            {
                if (((ListBoxItem)sender).DataContext is Playlist playlist)
                    vm.AddToPlaylistCommand.Execute(playlist);
            }
        }

        //SLIDER
        private void UpdatePosition(object? sender, EventArgs e)
        {
            if (_userIsDragging) return;
            var vm = (SoundViewModel)DataContext;
            if(vm != null) vm.Position = vm.AudioPlayerService.GetPositionSeconds();
        }
        private void Slider_DragEnter(object sender, DragStartedEventArgs e)
        {
            _userIsDragging = true;
            Debug.WriteLine("Started dragging!");
        }

        private void Slider_DragLeave(object sender, DragCompletedEventArgs e)
        {
            _userIsDragging = false;
            Debug.WriteLine("Ended dragging!");
            // Now push the final slider value to the player
            var vm = (SoundViewModel)DataContext;
            vm.AudioPlayerService.Seek(vm.Position);
        }
    }

}
