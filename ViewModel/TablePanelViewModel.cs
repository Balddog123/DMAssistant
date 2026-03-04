using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace DMAssistant.ViewModel
{
    public partial class TablePanelViewModel : ObservableObject
    {
        [ObservableProperty] ObservableCollection<Table> tables = new();
        public ICollectionView TablesView { get; }

        private Table selectedTable;
        public Table SelectedTable
        {
            get => selectedTable;
            set
            {
                SetProperty(ref selectedTable, value);
                SelectedTableView = new TableViewModel(value);
                HookItemEvents(SelectedTableView);
            }
        }
        [ObservableProperty] TableViewModel selectedTableView;

        private string _search = "";
        public string Search
        {
            get => _search;
            set
            {
                if (SetProperty(ref _search, value)) ApplyFilters();
            }
        }

        public IRelayCommand AddTableCommand { get; }
        public IRelayCommand DeleteTableCommand { get; }
        public TablePanelViewModel()
        {
            Tables = App.CampaignStore.CurrentCampaign.Tables;

            AddTableCommand = new RelayCommand(AddTable);
            DeleteTableCommand = new RelayCommand<Table>(deleteTable =>
            {
                if (MessageBox.Show($"Delete {deleteTable.Name}?",
                                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    Tables.Remove(deleteTable);
                }

            });

            TablesView = CollectionViewSource.GetDefaultView(Tables);
            TablesView.SortDescriptions.Add(new SortDescription(nameof(Table.IsNew), ListSortDirection.Descending));
            TablesView.SortDescriptions.Add(new SortDescription(nameof(Table.Name), ListSortDirection.Ascending));
            TablesView.Filter = FilterTable;
            ApplyFilters();
        }
        private void HookItemEvents(TableViewModel vm)
        {
            vm.PropertyChanged += (_, args) =>
            {
                // Example: react to name changes
                if (args.PropertyName == nameof(TableViewModel.Name))
                {
                    // Raise panel-level update (e.g., refresh list)
                    OnPropertyChanged(nameof(Tables));
                    ApplyFilters();
                }
            };
        }
        private void AddTable()
        {
            Debug.WriteLine("adding table...");
            Tables.Add(new Table() { IsNew = true });
        }
        private bool FilterTable(object obj)
        {
            if (obj is not Table m) return false;

            if (!string.IsNullOrWhiteSpace(Search) &&
                !m.Name.Contains(Search, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
        private void ApplyFilters()
        {
            TablesView.Refresh();
        }
        private TableViewModel CreateTableViewModel(Table table)
        {
            var vm = new TableViewModel(table);

            // listen to whenever Name/Rank/etc. changes
            HookTableEvents(vm);

            return vm;
        }

        private void HookTableEvents(TableViewModel vm)
        {
            vm.PropertyChanged += (_, args) =>
            {
                // Example: react to name changes
                if (args.PropertyName == nameof(TableViewModel.Name))
                {
                    // Raise panel-level update (e.g., refresh list)
                    OnPropertyChanged(nameof(Tables));
                }
            };
        }
    }
}
