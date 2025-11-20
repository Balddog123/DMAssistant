using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                    SelectedLocationViewModel = new LocationViewModel(_selectedLocation, this)
                    {
                        DeleteCommand = DeleteLocationCommand // parent VM command
                    };
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
        public IRelayCommand DeleteLocationCommand => new RelayCommand(async () =>
        {
            if (MessageBox.Show($"Delete {SelectedLocation.Name}?",
                                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var loc = SelectedLocation;

                // Clear the view model so bindings detach
                SelectedLocationViewModel = null;
                SelectedLocation = null;

                // Allow UI to update before deletion
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    App.CampaignStore.DeleteLocation(loc, _session);
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        });

        public LocationPanelViewModel(ObservableCollection<string> locationIDs, Session session)
        {
            _sessionLocationIDs = locationIDs;
            LocationList = new ObservableCollection<Location>();

            // Hydrate real Location objects
            foreach (string id in locationIDs)
            {
                if (App.CampaignStore.LocationIndex.TryGetValue(id, out var loc)) LocationList.Add(loc);
            }

            if (LocationList.Any()) SelectedLocation = LocationList[0];

            AddLocationCommand = new RelayCommand(AddLocation);
            AddExistingLocationCommand = new RelayCommand(AddExistingLocation);

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

        private void AddLocation()
        {
            var location = new Location();
            LocationList.Add(location);
            SelectedLocation = location;
            // Item belongs to global campaign list
            App.CampaignStore.CurrentCampaign.Locations.Add(location);
            App.CampaignStore.LocationIndex[location.ID] = location;
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
