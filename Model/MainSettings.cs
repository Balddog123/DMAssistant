using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public partial class MainSettings : ObservableObject
    {
        [ObservableProperty] public string lastCampaignFilePath = "";
        
    }
}
