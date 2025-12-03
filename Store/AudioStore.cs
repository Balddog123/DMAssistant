using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.Helpers;
using DMAssistant.Model;
using System.Diagnostics;

namespace DMAssistant.Store
{
    public class AudioStore : ObservableObject
    {
        public AudioSettings AudioSettings { get; set; }
        public AudioStore()
        {
            AudioSettings settings = SettingsSerializer.LoadSettings(SettingsSerializer.SettingsType.Audio) as AudioSettings;
            Debug.WriteLine($"Found {settings}");
            if (settings == null) AudioSettings = new AudioSettings();
            else AudioSettings = settings;
        }
    }
}
