using DMAssistant.Model;
using System.Collections.Generic;
using System.Windows;

namespace DMAssistant.View
{
    public partial class SelectNPCWindow : Window
    {
        public List<NPC> AvailableNPCs { get; }
        public NPC SelectedNPC { get; private set; }

        public SelectNPCWindow(List<NPC> availableNPCs)
        {
            InitializeComponent();
            AvailableNPCs = availableNPCs;
            NPCListBox.ItemsSource = AvailableNPCs;
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
