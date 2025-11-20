using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public class AudioQueueElement : ObservableObject
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        private AudioFile _file;
        public AudioFile File
        {
            get => _file;
            set => SetProperty(ref _file, value);
        }

        public AudioQueueElement(AudioFile file)
        {
            File = file;
        }
    }
}
