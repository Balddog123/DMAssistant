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
using System.Windows.Documents;

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
                    SelectedNPCViewModel = new NPCViewModel(_selectedNPC);
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
        public IRelayCommand DeleteNPC => new RelayCommand<NPC>(npcToDelete =>
        {
            if (MessageBox.Show($"Remove {npcToDelete.Name}?", "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                //NPCList.Remove(npcToDelete);
                App.CampaignStore.DeleteNPC(npcToDelete, this);
            }
        });

        public IRelayCommand DuplicateNPC { get; }

        public NPCPanelViewModel(ObservableCollection<string> npcIds, Session session)
        {
            _sessionNPCIds = npcIds;
            NPCList = new ObservableCollection<NPC>();

            if (session == null) NPCList = App.CampaignStore.CurrentCampaign.NPCs;
            else
            {
                // Hydrate real Monster objects
                foreach (string id in npcIds)
                {
                    if (App.CampaignStore.NPCIndex.TryGetValue(id, out var npc))
                        NPCList.Add(npc);
                }
            }
                

            if (NPCList.Any()) SelectedNPC = NPCList[0]; // default selection
            AddNPCCommand = new RelayCommand(() => AddNPC());
            AddExistingNPCCommand = new RelayCommand(AddExistingNPC);
            DuplicateNPC = new RelayCommand<NPC>(npc => AddNPC(npc));

            App.CampaignStore.NPCDeleted += OnNPCDeleted;
            _session = session;
        }
        private void OnNPCDeleted(NPCPanelViewModel model, NPC npc)
        {
            if (model != null && model != this) return;

            Debug.WriteLine($"Deleting NPC {npc.Name}");
            if (NPCList.Contains(npc))
                NPCList.Remove(npc);

            if (SelectedNPC == npc)
                SelectedNPC = NPCList.FirstOrDefault();
        }

        private void AddExistingNPC()
        {
            // Open a simple selection dialog
            Debug.WriteLine("available npcs:");
            var availableNPCs = App.CampaignStore.CurrentCampaign.NPCs.ToList();
            foreach (var npc in availableNPCs) Debug.WriteLine(npc);

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

        private void AddNPC(NPC npcToCopy = null)
        {
            NPC newNpc;
            if (npcToCopy == null) { 
                newNpc = new NPC();
                newNpc.Gender = NPC.GetRandomGender();
                //get location
                Location location = NPC.GetRandomLocation();
                if (location != null) newNpc.Home = location;
                //get religion
                //get race
                newNpc.Race = NPC.GetRandomRace();
                //generate name
                newNpc.Name = NPC.GetRandomName(newNpc.Gender);
                //create description
                newNpc.Description = new FlowDocument();
                newNpc.Description.Blocks.Add(new Paragraph(new Run(NPC.GetRandomDescription())));
                //create goal
                newNpc.Goal = new FlowDocument();
                newNpc.Goal.Blocks.Add(new Paragraph(new Run(NPC.GetRandomGoal())));
            }
            else newNpc = new NPC(npcToCopy);
            
            

            //first, we'll generate these PURELY random. Later, we will use influences from location, race, and religion to derive some of these values

            // NPC belongs to global campaign list
            int index = App.CampaignStore.CurrentCampaign.NPCs.IndexOf(npcToCopy) + 1;
            App.CampaignStore.CurrentCampaign.NPCs.Insert(index, newNpc);
            App.CampaignStore.NPCIndex[newNpc.ID] = newNpc;
            // Add ID to session
            _sessionNPCIds.Add(newNpc.ID);
            //live object to panel
            NPCList.Insert(NPCList.IndexOf(npcToCopy) + 1, newNpc);
            SelectedNPC = newNpc;
        }

        
    }
}
