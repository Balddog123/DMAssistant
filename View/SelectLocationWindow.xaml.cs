using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DMAssistant.View
{
    /// <summary>
    /// Interaction logic for SelectLocationWindow.xaml
    /// </summary>
    public partial class SelectLocationWindow : Window
    {
        public List<Location> AvailableLocations { get; }
        public Location SelectedLocation { get; private set; }
        private ICollectionView _locationsView;

        public SelectLocationWindow(List<Location> availableLocations)
        {
            InitializeComponent();
            AvailableLocations = availableLocations;
            _locationsView = CollectionViewSource.GetDefaultView(AvailableLocations);
            _locationsView.SortDescriptions.Add(new SortDescription(nameof(Location.Name), ListSortDirection.Ascending));
            _locationsView.Filter = FilterMonster;
            LocationListBox.ItemsSource = _locationsView;
        }

        private bool FilterMonster(object obj)
        {
            if (obj is not Location location)
                return false;

            string search = SearchTextBox.Text?.Trim() ?? "";

            if (string.IsNullOrEmpty(search))
                return true;

            return location.Name.Contains(search, System.StringComparison.OrdinalIgnoreCase);
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _locationsView.Refresh();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            SelectedLocation = (Location)LocationListBox.SelectedItem;
            if (SelectedLocation != null)
                DialogResult = true;
            else
                MessageBox.Show("Please select an Location.");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
