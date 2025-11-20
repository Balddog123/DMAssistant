using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DMAssistant.Model;
using DMAssistant.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DMAssistant.ViewModel
{
    public class NPCPanelViewModel : ObservableObject
    {
        private readonly ObservableCollection<string> _sessionNPCIds;
        public Session _session { get; private set; }
        public ObservableCollection<NPC> NPCList { get; }
        //get list of npcs from here: var campaign = App.CampaignStore.CurrentCampaign;

        private NPC _selectedNPC;
        public NPC SelectedNPC
        {
            get => _selectedNPC;
            set 
            {
                if (SetProperty(ref _selectedNPC, value))
                {
                    SelectedNPCViewModel = new NPCViewModel(_selectedNPC)
                    {
                        DeleteCommand = DeleteNPCCommand // parent VM command
                    };
                }
            }
        }
        private NPCViewModel _selectedNPCViewModel;
        public NPCViewModel SelectedNPCViewModel
        {
            get => _selectedNPCViewModel;
            set => SetProperty(ref _selectedNPCViewModel, value);
        }
        public IRelayCommand AddNPCCommand { get; }
        public IRelayCommand AddExistingNPCCommand { get; }
        public IRelayCommand DeleteNPCCommand => new RelayCommand(async () =>
        {
            if (MessageBox.Show($"Delete {SelectedNPC.Name}?",
                                "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var loc = SelectedNPC;

                // Clear the view model so bindings detach
                SelectedNPCViewModel = null;
                SelectedNPC = null;

                // Allow UI to update before deletion
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    App.CampaignStore.DeleteNPC(loc, _session);
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        });

        public NPCPanelViewModel(ObservableCollection<string> npcIds, Session session)
        {
            _sessionNPCIds = npcIds;
            NPCList = new ObservableCollection<NPC>();

            // Hydrate real Monster objects
            foreach (string id in npcIds)
            {
                if (App.CampaignStore.NPCIndex.TryGetValue(id, out var npc))
                    NPCList.Add(npc);
            }

            if (NPCList.Any()) SelectedNPC = NPCList[0]; // default selection
            AddNPCCommand = new RelayCommand(AddNPC);
            AddExistingNPCCommand = new RelayCommand(AddExistingNPC);

            App.CampaignStore.NPCDeleted += OnNPCDeleted;
            _session = session;
        }
        private void OnNPCDeleted(NPC npc)
        {
            if (NPCList.Contains(npc))
                NPCList.Remove(npc);

            if (SelectedNPC == npc)
                SelectedNPC = NPCList.FirstOrDefault();
        }

        private void AddExistingNPC()
        {
            // Open a simple selection dialog
            var availableNPCs = App.CampaignStore.CurrentCampaign.NPCs.ToList();

            if (!availableNPCs.Any())
            {
                MessageBox.Show("No NPCs available!");
                return;
            }

            var window = new SelectNPCWindow(availableNPCs);
            var result = window.ShowDialog();

            if (result == true && window.SelectedNPC != null)
            {
                _sessionNPCIds.Add(window.SelectedNPC.ID);
                NPCList.Add(window.SelectedNPC);
                SelectedNPC = window.SelectedNPC;
            }
        }

        private void AddNPC()
        {
            var newNpc = new NPC("Aric", "Human", "Wanderer", "Find treasure", "Riverfall");
            // NPC belongs to global campaign list
            App.CampaignStore.CurrentCampaign.NPCs.Add(newNpc);
            App.CampaignStore.NPCIndex[newNpc.ID] = newNpc;
            // Add ID to session
            _sessionNPCIds.Add(newNpc.ID);
            //live object to panel
            NPCList.Add(newNpc);
            SelectedNPC = newNpc;
        }
    }
}
