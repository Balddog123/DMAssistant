using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.Repository;
using DMAssistant.View;
using DMAssistant.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

public class MainWindowViewModel : ObservableObject
{
    public CampaignViewModel CampaignVM { get; }
    public SessionViewModel SessionVM { get; }
    public SoundPanelViewModel SoundPanelVM { get; }
    public object CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }
    private object _currentView;

    public ICommand ShowCampaignCommand { get; }
    public ICommand ShowSessionsCommand { get; }
    public ICommand ShowSoundPanelCommand { get; }

    public MainWindowViewModel()
    {
        // Create viewmodels ONCE
        CampaignVM = new CampaignViewModel();
        SessionVM = new SessionViewModel();
        SoundPanelVM = new SoundPanelViewModel();

        ShowCampaignCommand = new RelayCommand(() =>
        {
            CampaignVM.AccumulateIds();
            CurrentView = CampaignVM;
            CampaignVM.ResetView();
        });
        ShowSessionsCommand = new RelayCommand(() => CurrentView = SessionVM);
        ShowSoundPanelCommand = new RelayCommand(() => CurrentView = SoundPanelVM);
        CurrentView = CampaignVM; // default
    }
}

