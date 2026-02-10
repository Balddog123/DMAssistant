using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DMAssistant.ViewModel
{
    public partial class TableViewModel : ObservableObject
    {
        private Table table;
        public Table Table
        {
            get => table; set
            {
                SetProperty(ref table, value);
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Name
        {
            get => Table.Name;
            set
            {
                SetProperty(Table.Name, value, Table, (m, v) => m.Name = v);
            }
        }

        public ICommand AddTableItemCommand { get; }
        public ICommand RemoveTableItemCommand { get; }

        public TableViewModel(Table table)
        {
            Table = table;
            Debug.WriteLine($"Created a table view for {Table.Name}...");
            AddTableItemCommand = new RelayCommand(() =>
            {
                Table.Values.Add("");
                //OnPropertyChanged(nameof(Table));
            });
            RemoveTableItemCommand = new RelayCommand<string>(deleteItem =>
            {
                if (Table.Values.Contains(deleteItem))
                {
                    Table.Values.Remove(deleteItem);
                }

            });
        }
    }
}
