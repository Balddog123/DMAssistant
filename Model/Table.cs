using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public partial class Table : ObservableObject
    {
        [ObservableProperty] public string name = "New Table";
        private ObservableCollection<string> values = new ObservableCollection<string>();
        public ObservableCollection<string> Values
        {
            get => values;
            set => values = value;
        }
        public Table()
        {

        }
    }
}
