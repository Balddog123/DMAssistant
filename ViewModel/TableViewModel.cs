using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public string Description
        {
            get => Table.Description;
            set
            {
                SetProperty(Table.Description, value, Table, (m, v) => m.Description = v);
            }
        }

        public ICommand AddTableItemCommand { get; }
        public ICommand RemoveTableItemCommand { get; }
        public ICommand RollCommand { get; }

        public TableViewModel(Table table)
        {
            Table = table;

            AddTableItemCommand = new RelayCommand(() =>
            {
                Table.Values.Add(new TableItem());
                //OnPropertyChanged(nameof(Table));
            });
            RemoveTableItemCommand = new RelayCommand<TableItem>(deleteItem =>
            {
                if (Table.Values.Contains(deleteItem))
                {
                    Table.Values.Remove(deleteItem);
                }

            });
            RollCommand = new RelayCommand(Roll);
        }

        private void Roll()
        {
            foreach (var item in Table.Values)
            {
                item.IsRolled = false;
            }
            int index = new Random().Next(0, Table.Values.Count);
            Table.Values[index].IsRolled = true;
        }
    }
}
