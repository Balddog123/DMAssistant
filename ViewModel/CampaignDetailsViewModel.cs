using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.ViewModel
{
    public class CampaignDetailsViewModel : ObservableObject
    {
        public string Name
        {
            get => App.CampaignStore.CurrentCampaign?.Name ?? string.Empty;
            set
            {
                if (App.CampaignStore.CurrentCampaign != null)
                {
                    App.CampaignStore.CurrentCampaign.Name = value;
                    OnPropertyChanged(nameof(Name)); // Notify the view
                }
            }
        }

        public CampaignDetailsViewModel()
        {
        }
    }
}
