using DMAssistant.Model;
using System;
using System.Collections.Generic;
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

        public SelectLocationWindow(List<Location> availableLocations)
        {
            InitializeComponent();
            AvailableLocations = availableLocations;
            LocationListBox.ItemsSource = AvailableLocations;
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
