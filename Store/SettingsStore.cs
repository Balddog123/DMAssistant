using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.Helpers;
using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DMAssistant.Store
{
    public class SettingsStore : ObservableObject
    {
        public Settings Settings { get; set; }
        public SettingsStore()
        {
            Settings settings = SettingsSerializer.LoadSettings();
            if (settings == null) Settings = new Settings();
            else Settings = settings;
        }        
    }
}
