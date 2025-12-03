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
    /// Interaction logic for SelectMonsterWindow.xaml
    /// </summary>
    public partial class SelectMonsterWindow : Window
    {
        public List<Monster> AvailableMonsters { get; }
        public Monster SelectedMonster { get; private set; }
        private ICollectionView _monstersView;

        public SelectMonsterWindow(List<Monster> availableMonsters)
        {
            InitializeComponent();
            AvailableMonsters = availableMonsters;

            _monstersView = CollectionViewSource.GetDefaultView(AvailableMonsters);
            _monstersView.Filter = FilterMonster;
            MonsterListBox.ItemsSource = _monstersView;
        }

        private bool FilterMonster(object obj)
        {
            if (obj is not Monster monster)
                return false;

            string search = SearchTextBox.Text?.Trim() ?? "";

            if (string.IsNullOrEmpty(search))
                return true;

            return monster.Name.Contains(search, System.StringComparison.OrdinalIgnoreCase);
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _monstersView.Refresh();
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
