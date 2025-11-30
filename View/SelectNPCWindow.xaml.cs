using DMAssistant.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace DMAssistant.View
{
    public partial class SelectNPCWindow : Window
    {
        public List<NPC> AvailableNPCs { get; }
        public NPC SelectedNPC { get; private set; }

        private ICollectionView _NPCView;

        public SelectNPCWindow(List<NPC> availableNPCs)
        {
            InitializeComponent();
            AvailableNPCs = availableNPCs;

            _NPCView = CollectionViewSource.GetDefaultView(AvailableNPCs);
            _NPCView.Filter = FilterMonster;
            NPCListBox.ItemsSource = _NPCView;
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
            _NPCView.Refresh();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            SelectedNPC = (NPC)NPCListBox.SelectedItem;
            if (SelectedNPC != null)
                DialogResult = true;
            else
                MessageBox.Show("Please select an NPC.");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
