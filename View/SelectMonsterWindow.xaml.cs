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
    /// Interaction logic for SelectMonsterWindow.xaml
    /// </summary>
    public partial class SelectMonsterWindow : Window
    {
        public List<Monster> AvailableMonsters { get; }
        public Monster SelectedMonster { get; private set; }

        public SelectMonsterWindow(List<Monster> availableMonsters)
        {
            InitializeComponent();
            AvailableMonsters = availableMonsters;
            MonsterListBox.ItemsSource = AvailableMonsters;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            SelectedMonster = (Monster)MonsterListBox.SelectedItem;
            if (SelectedMonster != null)
                DialogResult = true;
            else
                MessageBox.Show("Please select a Monster.");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
