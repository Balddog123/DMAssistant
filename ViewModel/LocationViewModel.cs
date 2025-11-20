using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Helpers;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DMAssistant.ViewModel
{
    public class LocationViewModel : ObservableObject
    {
        private readonly Location _location;
        private readonly LocationPanelViewModel _panel;
        public IRelayCommand DeleteCommand { get; set; }
        public RelayCommand SetImageFromFile => new RelayCommand(() =>
        {
            byte[] imageData = FileGetter.LoadImageFromFile();
            if(imageData != null)
            {
                _location.ImageData = imageData;
                OnPropertyChanged(nameof(LocationImage));
            }
        });
        public RelayCommand SetImageFromClipboard => new RelayCommand(() =>
        {
            byte[] imageData = FileGetter.LoadImageFromClipboard();
            if (imageData != null)
            {
                _location.ImageData = imageData;
                OnPropertyChanged(nameof(LocationImage));
            }
        });
        public RelayCommand AddNewMapCommand => new RelayCommand(() =>
        {
            if(_location.Maps == null) _location.Maps = new ObservableCollection<Map>();
            _location.Maps.Add(new Map());
        });
        public RelayCommand<Map> OpenMapCommand => new RelayCommand<Map>(map =>
        {
            Debug.WriteLine(map == null ? "MAP IS NULL" : "MAP LOADED");
            if (map == null) return;

            var mapWindow = new MapWindow(map);
            mapWindow.Show();
        });

        public LocationViewModel(Location location, LocationPanelViewModel panel)
        {
            _location = location;
            _panel = panel;
        }

        // Bindable Name
        public string Name
        {
            get => _location != null ? _location.Name : string.Empty;
            set
            {
                if (_location.Name != value)
                {
                    _location.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        // Bindable Description
        public string Description
        {
            get => _location != null ? _location.Description : string.Empty;
            set
            {
                if (_location.Description != value)
                {
                    _location.Description = value;
                    OnPropertyChanged();
                }
            }
        }

        // Computed Image for WPF Image control
        public BitmapImage LocationImage
        {
            get
            {
                if (_location == null || _location.ImageData == null || _location.ImageData.Length == 0)
                    return null;

                var bitmap = new BitmapImage();
                using (var ms = new MemoryStream(_location.ImageData))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze(); // Make it cross-thread safe
                }
                return bitmap;
            }
        }

        public ObservableCollection<Map> Maps => _location != null ? _location.Maps : new ObservableCollection<Map>();
        

    }
}
