using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DMAssistant.ViewModel
{
    public class CampaignFrontsViewModel : ObservableObject
    {
        private ObservableCollection<string> _fronts;
        public ObservableCollection<string> Fronts
        {
            get => _fronts;
            set
            {
                if (SetProperty(ref _fronts, value))
                {
                    // Update the underlying Campaign object
                    App.CampaignStore.CurrentCampaign.Fronts = value;
                }
            }
        }

        public ICommand AddNewFrontCommand { get; }
        public ICommand RemoveFrontCommand { get; }
        public CampaignFrontsViewModel()
        {
            _fronts = App.CampaignStore.CurrentCampaign.Fronts;
            AddNewFrontCommand = new RelayCommand(CreateNewFront);
            RemoveFrontCommand = new RelayCommand<string>(RemoveFront);
        }

        private void RemoveFront(string? obj)
        {
            Fronts.Remove(obj);
        }

        private void CreateNewFront()
        {
            Fronts.Add("New Front:");
        }
    }
}
