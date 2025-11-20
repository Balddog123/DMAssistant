using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DMAssistant.Model;
using MahApps.Metro.Controls;

namespace DMAssistant.View
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }
    }
}

