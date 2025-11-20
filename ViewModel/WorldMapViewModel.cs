using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.ViewModel
{
    public class WorldMapViewModel : ObservableObject
    {
        private Map _map;
        public Map Map
        {
            get=> _map != null ? _map : new Map();
            set
            {
                if(_map != value)
                {
                    SetProperty(ref _map, value);
                }
            }
        }

        public RelayCommand OpenMapCommand => new RelayCommand(() =>
        {
            Debug.WriteLine(_map == null ? "MAP IS NULL" : "MAP LOADED");
            if (_map == null) return;

            var mapWindow = new MapWindow(_map);
            mapWindow.Show();
        });

        public WorldMapViewModel() 
        {
            _map = App.CampaignStore.CurrentCampaign.WorldMap;
        }
    }
}
