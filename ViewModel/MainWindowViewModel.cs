using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant;
using DMAssistant.Model;
using DMAssistant.Repository;
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
                CampaignVM.ResetView();
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
            MessageBox.Show($"Attempting to save {App.CampaignStore.CurrentCampaign.Name}");
            CampaignSerializer.SaveCampaign(App.CampaignStore.CurrentCampaign);
        });
        Exit = new RelayCommand(Application.Current.Shutdown);
        CurrentView = CampaignVM; // default
    }

    private void CreateNewCampaign()
    {
        Debug.WriteLine("Creating new campaign");
        Campaign newCampaign = new Campaign();
        App.CampaignStore.StoreCampaign();
        CurrentView = null;
    }

    private void OpenCampaignDialog()
    {
        // Create an OpenFileDialog
        OpenFileDialog openFileDialog = new OpenFileDialog
        {
            Filter = "JSON Files (*.json)|*.json",
            InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Campaigns"),
            Title = "Open Campaign"
        };

        // Show dialog
        bool? result = openFileDialog.ShowDialog();

        if (result == true)
        {
            string selectedFile = openFileDialog.FileName;

            try
            {
                // Load the campaign from the selected file
                Campaign? campaign = CampaignSerializer.LoadCampaign(selectedFile);

                if (campaign != null)
                {
                    App.CampaignStore.StoreCampaign(campaign);
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

