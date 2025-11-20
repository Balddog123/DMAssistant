using DMAssistant.Helpers;
using DMAssistant.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace DMAssistant
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static CampaignStore CampaignStore { get; } = new CampaignStore();

        protected override void OnExit(ExitEventArgs e)
        {
            CampaignSerializer.SaveCampaign(App.CampaignStore.CurrentCampaign);
            base.OnExit(e);
        }

    }

}
