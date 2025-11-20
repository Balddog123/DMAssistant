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
    /// Interaction logic for SelectItemWindow.xaml
    /// </summary>
    public partial class SelectItemWindow : Window
    {
        public List<Item> AvailableItems{ get; }
        public Item SelectedItem { get; private set; }

        public SelectItemWindow(List<Item> availableItems)
        {
            InitializeComponent();
            AvailableItems = availableItems;
            ItemListBox.ItemsSource = AvailableItems;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem = (Item)ItemListBox.SelectedItem;
            if (SelectedItem != null)
                DialogResult = true;
            else
                MessageBox.Show("Please select an Item.");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
