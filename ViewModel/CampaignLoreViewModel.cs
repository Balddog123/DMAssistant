using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.ViewModel
{
    public class CampaignLoreViewModel : ObservableObject
    {
        private string _lore;
        public string Lore
        {
            get => _lore;
            set
            {
                if (SetProperty(ref _lore, value))
                {
                    // Update the underlying Campaign object
                    App.CampaignStore.CurrentCampaign.Lore = value;
                }
            }
        }

        public CampaignLoreViewModel()
        {
            _lore = App.CampaignStore.CurrentCampaign.Lore;
        }
    }
}
