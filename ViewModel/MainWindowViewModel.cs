using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant;
using DMAssistant.Helpers;
using DMAssistant.Model;
using DMAssistant.Repository;
using DMAssistant.Services;
using DMAssistant.Store;
using DMAssistant.View;
using DMAssistant.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
    public ICommand OpenCampaign { get; }
    public ICommand NewCampaign { get; }
    public ICommand SaveCampaign { get; }
    public ICommand Exit { get; }

    //

    public ICommand ResetMonsters { get; }

    public string CampaignName
    {
        get => App.CampaignStore?.CurrentCampaign?.Name ?? string.Empty;
    }

    public MainWindowViewModel()
    {
        // Create viewmodels ONCE
        CampaignVM = new CampaignViewModel();
        SessionVM = new SessionViewModel();
        SoundPanelVM = new SoundPanelViewModel();

        ShowCampaignCommand = new RelayCommand(() =>
        {
            if(App.CampaignStore.CurrentCampaign != null)
            {
                CampaignVM.AccumulateIds();
                CurrentView = CampaignVM;
                //CampaignVM.ResetView();
            }            
        });
        ShowSessionsCommand = new RelayCommand(() =>
        {
            if (App.CampaignStore.CurrentCampaign != null)
            {
                CurrentView = SessionVM;
            }
        });
        ShowSoundPanelCommand = new RelayCommand(() => CurrentView = SoundPanelVM);

        OpenCampaign = new RelayCommand(OpenCampaignDialog);
        NewCampaign = new RelayCommand(CreateNewCampaign);
        SaveCampaign = new RelayCommand(() =>
        {
            MessageBox.Show($"Saved {App.CampaignStore.CurrentCampaign.Name}!");
            CampaignSerializer.SaveCampaign(App.CampaignStore.CurrentCampaign);
            SettingsSerializer.SaveSettings(App.SettingsStore.Settings, SettingsSerializer.SettingsType.Main);
            App.AudioStore.AudioSettings.musicVolume = MainWindow.MusicPlayer.Player.GetVolume;
            App.AudioStore.AudioSettings.ambienceVolume = MainWindow.AmbiencePlayer.Player.GetVolume;
            App.AudioStore.AudioSettings.soundVolume = MainWindow.SoundPlayer.Player.GetVolume;
            SettingsSerializer.SaveSettings(App.AudioStore.AudioSettings, SettingsSerializer.SettingsType.Audio);
        });
        Exit = new RelayCommand(Application.Current.Shutdown);
        CurrentView = CampaignVM; // default

        //

        ResetMonsters = new RelayCommand(() =>
        {
            if (MessageBox.Show($"Are you sure you want to reset all monster data?\nThere will be an attempt to reconnect monsters in your sessions, but if there is a failure to reconnect,\n those monsters will be removed from the session data.",
                                "Reset", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                App.CampaignStore.ResetMonsters();
            }
        });

    }

    private void CreateNewCampaign()
    {
        Campaign newCampaign = new Campaign();
        App.CampaignStore.StoreCampaign();
        CurrentView = null;
        OnPropertyChanged(nameof(CampaignName));
    }

    private void OpenCampaignDialog()
    {
        Debug.WriteLine("----OpenCampaignDialog----");
        // Create an OpenFileDialog
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "JSON Files (*.json)|*.json",
            InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Campaigns"),
            Title = "Open Campaign"
        };

        // Show dialog
        bool? result = openFileDialog.ShowDialog();
        Debug.WriteLine($"Result: {result}");

        if (result == true)
        {
            string selectedFile = openFileDialog.FileName;
            Debug.WriteLine($"Selected file: {selectedFile}");
            try
            {
                // Load the campaign from the selected file
                Campaign? campaign = CampaignSerializer.LoadCampaign(selectedFile);

                if (campaign != null)
                {
                    App.CampaignStore.StoreCampaign(campaign);
                    OnPropertyChanged(nameof(CampaignName));
                }
                else
                {
                    MessageBox.Show("Failed to load campaign. File may be empty or corrupted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading campaign: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

