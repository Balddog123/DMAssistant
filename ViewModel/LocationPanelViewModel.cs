using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DMAssistant.ViewModel
{
    public class LocationPanelViewModel : ObservableObject
    {
        private readonly ObservableCollection<string> _sessionLocationIDs;
        public Session _session { get; private set; }
        public ObservableCollection<Location> LocationList { get; }

        //FILTERING
        public ICollectionView LocationsView { get; }
        private string _search = "";
        public string Search
        {
            get => _search;
            set
            {
                if (SetProperty(ref _search, value)) ApplyFilters();
            }
        }

        private Location _selectedLocation;
        public Location SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                if (SetProperty(ref _selectedLocation, value))
                {
                    var vm = new LocationViewModel(_selectedLocation, this);
                    HookItemEvents(vm);
                    SelectedLocationViewModel = vm;
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
            if (MessageBox.Show($"Delete {locationToDelete.Name}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.CampaignStore.DeleteLocation(locationToDelete, this);
            }
        });
        public RelayCommand<Location> DuplicateLocation { get; }

        public LocationPanelViewModel(ObservableCollection<string> locationIDs, Session session)
        {
            _sessionLocationIDs = locationIDs;            

            if (session == null) LocationList = App.CampaignStore.CurrentCampaign.Locations;
            else
            {
                LocationList = new ObservableCollection<Location>();

                // Hydrate real Location objects
                foreach (string id in locationIDs)
                {
                    if (App.CampaignStore.LocationIndex.TryGetValue(id, out var loc)) LocationList.Add(loc);
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

            LocationsView = CollectionViewSource.GetDefaultView(LocationList);
            LocationsView.SortDescriptions.Add(new SortDescription(nameof(Location.IsNew), ListSortDirection.Descending));
            LocationsView.SortDescriptions.Add(new SortDescription(nameof(Location.Name), ListSortDirection.Ascending));
            LocationsView.Filter = FilterItems;
            ApplyFilters();
        }

        private bool FilterItems(object obj)
        {
            if (obj is not Location m) return false;

            if (!string.IsNullOrWhiteSpace(Search) &&
                !m.Name.Contains(Search, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
        private void ApplyFilters()
        {
            LocationsView.Refresh();
        }

        private void HookItemEvents(LocationViewModel vm)
        {
            vm.PropertyChanged += (_, args) =>
            {
                // Example: react to name changes
                if (args.PropertyName == nameof(LocationViewModel.Name))
                {
                    // Raise panel-level update (e.g., refresh list)
                    OnPropertyChanged(nameof(LocationList));
                    ApplyFilters();
                }
            };
        }

        private void OnLocationDeleted(LocationPanelViewModel model, Location location)
        {
            if (model != null && model != this) return;

            Debug.WriteLine("Calling OnLocationDeleted event...");
            if (LocationList.Contains(location)) {
                LocationList.Remove(location);
            }

            if (SelectedLocation == location)
                SelectedLocation = LocationList.FirstOrDefault();
        }

        private void AddLocation(Location locationToCopy = null)
        {
            
            var location = locationToCopy != null ? new Location(locationToCopy) { IsNew = true} : new Location() { IsNew = true };
            int index = App.CampaignStore.CurrentCampaign.Locations.IndexOf(locationToCopy) + 1;

            Debug.WriteLine($"Location to copy: {locationToCopy}");
            LocationList.Insert(LocationList.IndexOf(locationToCopy) + 1, location);
            SelectedLocation = location;

            if(_session != null)
            {
                App.CampaignStore.CurrentCampaign.Locations.Insert(index, location);
                App.CampaignStore.LocationIndex[location.ID] = location;

                Debug.WriteLine($"Location successfully added to LocationsList: {App.CampaignStore.CurrentCampaign.Locations[index]}");
                Debug.WriteLine($"Location successfully added to index: {App.CampaignStore.LocationIndex[location.ID]}");
                // Add ID to session
                _sessionLocationIDs.Add(location.ID);
            }
            
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
