using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public class AudioFile : ObservableObject
    {
        private string _filePath = string.Empty;
        public string FilePath 
        { 
            get => _filePath; 
            set
            {
                if(_filePath != value)
                {
                    SetProperty(ref _filePath, value);
                    OnPropertyChanged(nameof(FilePath));
                }
            }
        }
        public string FileName => System.IO.Path.GetFileName(FilePath);
    }

}
