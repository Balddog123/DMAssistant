using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.ViewModel
{
    public class CampaignNotesViewModel : ObservableObject
    {
        private string _campaignNotes;
        public string CampaignNotes
        {
            get => _campaignNotes;
            set
            {
                if (SetProperty(ref _campaignNotes, value))
                {
                    // Update the underlying Campaign object
                    App.CampaignStore.CurrentCampaign.Notes = value;
                }
            }
        }

        private string _sessionNotes;
        public string SessionNotes
        {
            get => _sessionNotes;
            set => SetProperty(ref _sessionNotes, value);
        }

        public CampaignNotesViewModel()
        {
            _campaignNotes = App.CampaignStore.CurrentCampaign.Notes;
            _sessionNotes = App.CampaignStore.CurrentCampaign.GetAggregateSessionNotes();
        }
    }
}
