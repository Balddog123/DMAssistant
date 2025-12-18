using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DMAssistant.View
{
    /// <summary>
    /// Interaction logic for NPCPanelView.xaml
    /// </summary>
    public partial class NPCPanelView : UserControl
    {
        private TextBlock _lastBlockClicked;
        private ListSortDirection _lastDirection;

        public NPCPanelView()
        {
            InitializeComponent();
        }

        private void NPCListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not TextBlock block) return;

            var sortBy = block.Text;

            if (sortBy.ToLower() == "home") sortBy = "Home.Name";
            else if (sortBy.ToLower() == "name") sortBy = "Name";
            else return;

            ListSortDirection direction = ListSortDirection.Ascending;
            if (block == _lastBlockClicked && _lastDirection == ListSortDirection.Ascending)
                direction = ListSortDirection.Descending;

            ICollectionView view = CollectionViewSource.GetDefaultView(NPCListView.ItemsSource);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(new SortDescription(sortBy, direction));
            view.Refresh();

            _lastBlockClicked = block;
            _lastDirection = direction;
        }
    }
}
