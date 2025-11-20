using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.ViewModel
{
    public class CampaignFrontsViewModel : ObservableObject
    {
        private string _fronts;
        public string Fronts
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

        public CampaignFrontsViewModel()
        {
            _fronts = App.CampaignStore.CurrentCampaign.Fronts;
        }
    }
}
