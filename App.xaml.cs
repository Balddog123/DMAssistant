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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SettingsStore = new SettingsStore();
            Debug.WriteLine($"Settings: {SettingsStore.Settings}");
            Debug.WriteLine($"Settings, last campaign: {SettingsStore.Settings.LastCampaignFilePath}");

            CampaignStore = new CampaignStore();

            if (SettingsStore.Settings.LastCampaignFilePath != string.Empty)
            {
                Debug.WriteLine($"Attempting to fetch: {SettingsStore.Settings.LastCampaignFilePath}");
                Campaign? campaign = CampaignSerializer.LoadCampaign(SettingsStore.Settings.LastCampaignFilePath + ".json");
                Debug.WriteLine($"Fetched: {campaign.Name}");
               if(campaign != null) CampaignStore.StoreCampaign(campaign);
               else CampaignStore.StoreCampaign();
            }
            else
            {
                CampaignStore.StoreCampaign();
            }

                

            Debug.WriteLine($"Campaign Store, current campaign: {CampaignStore.CurrentCampaign.Name}");

        }

        protected override void OnExit(ExitEventArgs e)
        {
            CampaignSerializer.SaveCampaign(CampaignStore.CurrentCampaign);
            SettingsStore.Settings.LastCampaignFilePath = CampaignStore.CurrentCampaign != null ? 
                Path.Combine(CampaignSerializer.CampaignsFolderPath, CampaignStore.CurrentCampaign.Name) :
                string.Empty;
            SettingsSerializer.SaveSettings(SettingsStore.Settings);
            base.OnExit(e);
        }

    }

}
