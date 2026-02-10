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
        [ObservableProperty] ObservableCollection<TableViewModel> tableViewModels = new();
        public ICollectionView TablesView { get; }

        private TableViewModel selectedTable;
        public TableViewModel SelectedTable
        {
            get => selectedTable;
            set
            {
                SetProperty(ref selectedTable, value);
                Debug.WriteLine($"Selected table {selectedTable.Name}");
            }
        }

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
            TableViewModels = new ObservableCollection<TableViewModel>(
                App.CampaignStore.CurrentCampaign.Tables.Select(table => CreateTableViewModel(table))
            );

            AddTableCommand = new RelayCommand(() => AddTable());
            DeleteTableCommand = new RelayCommand<TableViewModel>(deleteTable =>
            {
                if (MessageBox.Show($"Delete {deleteTable.Name}?",
                                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    TableViewModels.Remove(deleteTable);
                }

            });

            TablesView = CollectionViewSource.GetDefaultView(TableViewModels);
            TablesView.Filter = FilterTable;
            ApplyFilters();
        }
        private void AddTable()
        {
            TableViewModels.Add(CreateTableViewModel(new Table()));
        }
        private bool FilterTable(object obj)
        {
            if (obj is not TableViewModel m) return false;

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
                    OnPropertyChanged(nameof(TableViewModels));
                }
            };
        }
    }
}
