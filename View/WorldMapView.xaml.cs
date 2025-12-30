using DMAssistant.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DMAssistant.View
{
    /// <summary>
    /// Interaction logic for WorldMapView.xaml
    /// </summary>
    public partial class WorldMapView : UserControl
    {
        public WorldMapView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
                return;

            if (DataContext is not WorldMapViewModel vm)
                return;

            vm.ZoomLevel += e.Delta > 0 ? 0.1 : -0.1;
            e.Handled = true;
        }

    }
}
