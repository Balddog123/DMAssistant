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
    /// <summary>
    /// Interaction logic for SoundPanel.xaml
    /// </summary>
    public partial class SoundPanelView : UserControl
    {
        private DispatcherTimer _timer;
        private bool _userIsDragging = false;
        private bool _isLooping = false;

        public SoundPanelView()
        {
            InitializeComponent();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += UpdatePosition;

            DataContextChanged += SoundPanelView_DataContextChanged;
        }

        private void SoundPanelView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is SoundPanelViewModel vm)
            {
                Debug.WriteLine("ViewModel attached!");

                vm.PlayAudioRequested += file =>
                {
                    Debug.WriteLine($"Attempting to play: {file}");
                    Player.Source = new Uri(file);
                    Player.Play();
                };

                vm.PauseRequested += () => Player.Pause();
                vm.StopRequested += () => Player.Stop();
                vm.ResumeRequested += () => Player.Play();
                vm.LoopRequested += () => _isLooping = !_isLooping;
                vm.VolumeChanged += volume => Player.Volume = volume;
            }
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (Player.NaturalDuration.HasTimeSpan)
            {
                var vm = (SoundPanelViewModel)DataContext;
                vm.Duration = Player.NaturalDuration.TimeSpan.TotalSeconds;
            }

            _timer.Start();
        }        

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_isLooping)
            {
                Player.Position = TimeSpan.FromSeconds(0);
            }
            else
            {

                Player.Stop();
                var vm = DataContext as SoundPanelViewModel;
                if (vm != null)
                {
                    vm.HandleAudioEnded();
                }
            }
        }

        private void AudioList_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine($"Double clicked!");

            if (sender is ListBox lb && lb.SelectedItem is AudioFile audio)
            {
                var vm = DataContext as SoundPanelViewModel;

                if (vm != null)
                {
                    Debug.WriteLine($"Double clicked! {audio.FilePath}");
                    vm.SelectedAudio = audio;
                    vm.Play(audio);
                    vm.AudioQueue.Insert(0, new AudioQueueElement(audio));
                }
            }
        }

        private void PlaylistItemClicked(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is SoundPanelViewModel vm)
            {
                if (((ListBoxItem)sender).DataContext is Playlist playlist)
                    vm.AddToPlaylistCommand.Execute(playlist);
            }
        }

        //SLIDER
        private void UpdatePosition(object? sender, EventArgs e)
        {
            if (_userIsDragging) return;
            var vm = (SoundPanelViewModel)DataContext;
            vm.Position = Player.Position.TotalSeconds;
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
            var vm = (SoundPanelViewModel)DataContext;
            Player.Position = TimeSpan.FromSeconds(vm.Position);
        }
    }

}
