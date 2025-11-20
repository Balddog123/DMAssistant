using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace DMAssistant.Model
{

    public class Playlist : ObservableObject
    {
        private string _name = "New Playlist";
        public string Name
        {
            get => _name; 
            set
            {
                if (_name != value)
                {
                    SetProperty(ref  _name, value);
                }
            }
        }
        private ObservableCollection<AudioFile> _files = new ObservableCollection<AudioFile>();
        public ObservableCollection<AudioFile> Files { get => _files;
            set
            {
                if (_files != value)
                {
                    SetProperty(ref _files, value);
                }
            }
        } 
    }

}
