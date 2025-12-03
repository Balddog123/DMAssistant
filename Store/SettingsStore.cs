using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.Helpers;
using DMAssistant.Model;

namespace DMAssistant.Store
{
    public class SettingsStore : ObservableObject
    {
        public MainSettings Settings { get; set; }
        public SettingsStore()
        {
            MainSettings settings = SettingsSerializer.LoadSettings(SettingsSerializer.SettingsType.Main) as MainSettings;
            if (settings == null) Settings = new MainSettings();
            else Settings = settings;
        }        
    }
}
