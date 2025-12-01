using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public partial class Settings : ObservableObject
    {
        [ObservableProperty] public string lastCampaignFilePath = "";
        [ObservableProperty] public string musicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Music");
    }
}
