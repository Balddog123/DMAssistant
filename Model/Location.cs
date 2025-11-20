using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public class Location : ObservableObject
    {
        public string ID { get; set; } = Guid.NewGuid().ToString();
        private string _name = "New Location";
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _description = "Description of New Location";
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private byte[]? _imageData;
        public byte[]? ImageData { get=> _imageData; set => SetProperty(ref _imageData, value); }

        private ObservableCollection<Map> _maps;
        public ObservableCollection<Map> Maps { get => _maps; set => SetProperty(ref _maps, value); }

        public Location()
        {

        }
    }
}
