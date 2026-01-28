using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DMAssistant.ViewModel
{
    public class LocationPanelViewModel : ObservableObject
    {
        private readonly ObservableCollection<string> _sessionLocationIDs;
        public Session _session { get; private set; }
        public ObservableCollection<Location> LocationList { get; }

        private Location _selectedLocation;
        public Location SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                if (SetProperty(ref _selectedLocation, value))
                {
                    SelectedLocationViewModel = new LocationViewModel(_selectedLocation, this);
                }
            }
        }

        private LocationViewModel _selectedLocationViewModel;
        public LocationViewModel SelectedLocationViewModel
        {
            get => _selectedLocationViewModel;
            set => SetProperty(ref _selectedLocationViewModel, value);
        }

        public IRelayCommand AddLocationCommand { get; }
        public IRelayCommand AddExistingLocationCommand { get; }
        public IRelayCommand DeleteLocation => new RelayCommand<Location>(locationToDelete =>
        {
            if (MessageBox.Show($"Delete {locationToDelete.Name}?",
                                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.CampaignStore.DeleteLocation(locationToDelete, _session);
            }
        });
        public RelayCommand<Location> DuplicateLocation { get; }

        public LocationPanelViewModel(ObservableCollection<string> locationIDs, Session session)
        {
            _sessionLocationIDs = locationIDs;
            LocationList = new ObservableCollection<Location>();

            // Hydrate real Location objects
            Debug.WriteLine($"Number of locations in Campaign: {App.CampaignStore.CurrentCampaign.Locations.Count}");
            foreach (string id in locationIDs)
            {
                if (App.CampaignStore.LocationIndex.TryGetValue(id, out var loc)) LocationList.Add(loc);
                else
                {
                    Debug.WriteLine($"Could not get Location value: {id}");
                }
            }

            if (LocationList.Any()) SelectedLocation = LocationList[0];

            AddLocationCommand = new RelayCommand(() => AddLocation());
            AddExistingLocationCommand = new RelayCommand(AddExistingLocation);
            DuplicateLocation = new RelayCommand<Location>(location =>
            {
                AddLocation(location);
            });

            App.CampaignStore.LocationDeleted += OnLocationDeleted;
            _session = session;
        }

        private void OnLocationDeleted(Location location)
        {
            if (LocationList.Contains(location))
                LocationList.Remove(location);

            if (SelectedLocation == location)
                SelectedLocation = LocationList.FirstOrDefault();
        }

        private void AddLocation(Location locationToCopy = null)
        {
            
            var location = locationToCopy != null ? new Location(locationToCopy) : new Location();
            int index = App.CampaignStore.CurrentCampaign.Locations.IndexOf(locationToCopy) + 1;

            Debug.WriteLine($"Location to copy: {locationToCopy}");
            LocationList.Insert(LocationList.IndexOf(locationToCopy) + 1, location);
            SelectedLocation = location;

            App.CampaignStore.CurrentCampaign.Locations.Insert(index, location);
            App.CampaignStore.LocationIndex[location.ID] = location;

            Debug.WriteLine($"Location successfully added to LocationsList: {App.CampaignStore.CurrentCampaign.Locations[index]}");
            Debug.WriteLine($"Location successfully added to index: {App.CampaignStore.LocationIndex[location.ID]}");
            // Add ID to session
            _sessionLocationIDs.Add(location.ID);
        }

        private void AddExistingLocation()
        {
            // Open a simple selection dialog
            var available = App.CampaignStore.CurrentCampaign.Locations.ToList();

            if (!available.Any())
            {
                MessageBox.Show("No Locations available!");
                return;
            }

            var window = new SelectLocationWindow(available);
            var result = window.ShowDialog();

            if (result == true && window.SelectedLocation != null)
            {
                _sessionLocationIDs.Add(window.SelectedLocation.ID);
                LocationList.Add(window.SelectedLocation);
                SelectedLocation = window.SelectedLocation;
            }
        }
    }

}
