using DMAssistant.Helpers;
using DMAssistant.Model;
using DMAssistant.Services;
using DMAssistant.Store;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace DMAssistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static CampaignStore CampaignStore { get; private set; } 
        public static SettingsStore SettingsStore { get; private set; }
        public static AudioStore AudioStore { get; private set; } 

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SettingsStore = new SettingsStore();
            AudioStore = new AudioStore();
            CampaignStore = new CampaignStore();

            if (SettingsStore.Settings.LastCampaignFilePath != string.Empty)
            {
               Campaign? campaign = CampaignSerializer.LoadCampaign(SettingsStore.Settings.LastCampaignFilePath + ".json");
               if(campaign != null) CampaignStore.StoreCampaign(campaign);
               else CampaignStore.StoreCampaign();
            }
            else
            {
                CampaignStore.StoreCampaign();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            CampaignSerializer.SaveCampaign(CampaignStore.CurrentCampaign);
            SettingsStore.Settings.LastCampaignFilePath = CampaignStore.CurrentCampaign != null ? 
                Path.Combine(CampaignSerializer.CampaignsFolderPath, CampaignStore.CurrentCampaign.Name) :
                string.Empty;
            SettingsSerializer.SaveSettings(SettingsStore.Settings, SettingsSerializer.SettingsType.Main);
            SettingsSerializer.SaveSettings(AudioStore.AudioSettings, SettingsSerializer.SettingsType.Audio);
            base.OnExit(e);
        }


    }

}
