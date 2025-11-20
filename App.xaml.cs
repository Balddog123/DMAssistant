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

            Campaign? campaign = new Campaign();
            if (SettingsStore.Settings.LastCampaignFilePath != string.Empty)
            {
                Debug.WriteLine($"Attempting to fetch: {SettingsStore.Settings.LastCampaignFilePath}");
                campaign = CampaignSerializer.LoadCampaign(SettingsStore.Settings.LastCampaignFilePath + ".json");
                Debug.WriteLine($"Fetched: {campaign.Name}");
            }

            CampaignStore.StoreCampaign(campaign);

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
