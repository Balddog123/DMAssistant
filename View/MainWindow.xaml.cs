using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DMAssistant.Model;

namespace DMAssistant.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }
    }
}

