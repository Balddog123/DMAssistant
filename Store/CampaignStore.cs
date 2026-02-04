using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.Helpers;
using DMAssistant.Model;
using DMAssistant.Repository;
using DMAssistant.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DMAssistant.Services
{
    public class CampaignStore : ObservableObject
    {
        private Campaign _campaign;
        public Campaign CurrentCampaign
        {
            get => _campaign;
            set => SetProperty(ref _campaign, value);
        }

        public Dictionary<string, Monster> MonsterIndex { get; private set; } = new Dictionary<string, Monster>();
        public event Action<Monster> MonsterDeleted;
        public Dictionary<string, NPC> NPCIndex { get; private set; } = new Dictionary<string, NPC>();
        public event Action<NPCPanelViewModel, NPC> NPCDeleted;
        public Dictionary<string, Item> ItemIndex { get; private set; } = new Dictionary<string, Item>();
        public event Action<ItemPanelViewModel, Item> ItemDeleted;
        public Dictionary<string, Location> LocationIndex { get; private set; } = new Dictionary<string, Location>();
        public event Action<LocationPanelViewModel, Location> LocationDeleted;

        public CampaignStore()
        {
        }

        public void StoreCampaign(Campaign? campaign = null)
        {
            if (campaign == null)
            {
                campaign = new Campaign();
            }
            else
            {
                //checks to update old data
                if (campaign.WorldMap == null) campaign.WorldMap = new Map();
            }

            if (campaign.Monsters.Count == 0) campaign.SetMonsters(new ObservableCollection<Monster>(DataRepository.GetAllMonsters()));
            if(campaign.Spells.Count == 0) campaign.Spells = new ObservableCollection<Spell>(DataRepository.GetAllSpells());

            MonsterIndex = campaign.Monsters.ToDictionary(m => m.ID, m => m);
            NPCIndex = campaign.NPCs.ToDictionary(m => m.ID, m => m);
            ItemIndex = campaign.Items.ToDictionary(m => m.ID, m => m);
            LocationIndex = campaign.Locations.ToDictionary(m => m.ID, m => m);
            CurrentCampaign = campaign;
        }

        public void DeleteLocation(Location location, LocationPanelViewModel locationPanel)
        {
            if (location == null) return;

            if (locationPanel._session != null)
            {
                foreach (var session in CurrentCampaign.Sessions)
                {
                    if (locationPanel._session == session && session.LocationIDs.Contains(location.ID)) session.LocationIDs.Remove(location.ID);
                }

                LocationDeleted?.Invoke(locationPanel, location);

            }
            else
            {
                foreach (var loc in CurrentCampaign.Locations)
                {
                    if (loc.ID == location.ID)
                    {
                        CurrentCampaign.Locations.Remove(location);
                        break;
                    }
                }

                // Remove from the LocationIndex dictionary
                if (LocationIndex.ContainsKey(location.ID))
                {
                    LocationIndex.Remove(location.ID);
                }

                foreach (var session in CurrentCampaign.Sessions)
                {
                    if (session.LocationIDs.Contains(location.ID)) session.LocationIDs.Remove(location.ID);
                }

                LocationDeleted?.Invoke(null, location);

            }

        }
        public void DeleteNPC(NPC npc, NPCPanelViewModel npcPanel)
        {
            if (npc == null) return;

            if (npcPanel._session != null)
            {
                //remove just the reference from a single session
                foreach (var session in CurrentCampaign.Sessions)
                {
                    if (npcPanel._session == session && session.NPCIDs.Contains(npc.ID)) session.NPCIDs.Remove(npc.ID);
                }
                NPCDeleted?.Invoke(npcPanel, npc);
            }
            else
            {
                //remove from EVERYTHING!

                //campaign data
                foreach (var _npc in CurrentCampaign.NPCs)
                {
                    if (_npc.ID == npc.ID)
                    {
                        CurrentCampaign.NPCs.Remove(npc);
                        break;
                    }
                }

                // Remove from the LocationIndex dictionary
                if (NPCIndex.ContainsKey(npc.ID))
                {
                    NPCIndex.Remove(npc.ID);
                }

                //ID references
                foreach (var session in CurrentCampaign.Sessions)
                {
                    if (session.NPCIDs.Contains(npc.ID)) session.NPCIDs.Remove(npc.ID);
                }
                NPCDeleted?.Invoke(null, npc);
            }
            
        }
        public void DeleteItem(Item item, ItemPanelViewModel itemPanel)
        {
            if (item == null) return;

            if (itemPanel._session != null)
            {
                //remove just the reference from a single session
                foreach (var session in CurrentCampaign.Sessions)
                {
                    if (itemPanel._session == session && session.ItemIDs.Contains(item.ID)) session.ItemIDs.Remove(item.ID);
                }
            }
            else
            {
                //campaign data
                foreach (var _item in CurrentCampaign.Items)
                {
                    if (_item.ID == item.ID)
                    {
                        CurrentCampaign.Items.Remove(item);
                        break;
                    }
                }

                // Remove from the LocationIndex dictionary
                if (ItemIndex.ContainsKey(item.ID))
                {
                    ItemIndex.Remove(item.ID);
                }

                //ID references
                foreach (var session in CurrentCampaign.Sessions)
                {
                    if (session.ItemIDs.Contains(item.ID)) session.ItemIDs.Remove(item.ID);
                }

                ItemDeleted?.Invoke(itemPanel, item);

            }

        }
        public void DeleteMonster(Monster monster, Session removeFromSession)
        {
            if (monster == null) return;

            if (removeFromSession != null)
            {
                //remove just the reference from a single session
                foreach (var session in CurrentCampaign.Sessions)
                {
                    //if (removeFromSession == session && session.MonsterIDs.Contains(monster.ID)) session.MonsterIDs.Remove(monster.ID);
                }
            }
            else
            {
                //remove from EVERYTHING!

                //campaign data
                foreach (var _monster in CurrentCampaign.Monsters)
                {
                    if (_monster.ID == monster.ID)
                    {
                        CurrentCampaign.Monsters.Remove(monster);
                        break;
                    }
                }

                // Remove from the LocationIndex dictionary
                if (MonsterIndex.ContainsKey(monster.ID)) MonsterIndex.Remove(monster.ID);

                //ID references
                foreach (var session in CurrentCampaign.Sessions)
                {
                    //if (session.MonsterIDs.Contains(monster.ID)) session.MonsterIDs.Remove(monster.ID);
                }
            }

            MonsterDeleted?.Invoke(monster);
        }

        public void ResetMonsters()
        {
            Dictionary<string, Monster> nameMonsterPairs = new Dictionary<string, Monster>();            

            ObservableCollection<Monster> newMonsters = new ObservableCollection<Monster>(DataRepository.GetAllMonsters());
            foreach (Monster monster in newMonsters)
            {
                nameMonsterPairs[monster.Name] = monster;
            }

            foreach (Session session in CurrentCampaign.Sessions)
            {
                foreach(Encounter encounter in session.Encounters)
                {
                    foreach(EncounterItem encounterItem in encounter.EncounterItems)
                    {
                        if(encounterItem is MonsterGroup mg)
                        {
                            //get name of old data
                            MonsterIndex.TryGetValue(mg.monsterId, out Monster monster);
                            //compare with new
                            if (monster != null)
                            {
                                nameMonsterPairs.TryGetValue(monster.Name, out Monster matchingMonster);
                                if (matchingMonster != null)
                                {
                                    mg.MonsterId = matchingMonster.ID;
                                }
                            }
                        }
                    }
                }
            }

            CurrentCampaign.SetMonsters(newMonsters);
            MonsterIndex = CurrentCampaign.Monsters.ToDictionary(m => m.ID, m => m);
        }
        public void AddSRDItems()
        {
            ObservableCollection<Item> srdItems = new ObservableCollection<Item>(DataRepository.GetSRDItems());
            Dictionary<string, Item> myItemsNames = ItemIndex.Values.ToDictionary(i => i.Name, i => i);

            for (int i = 0; i < srdItems.Count;)
            {
                if (myItemsNames.TryGetValue(srdItems[i].Name, out Item matchedItem))
                {
                    srdItems.Remove(srdItems[i]);
                }
                else
                {
                    CurrentCampaign.Items.Add(srdItems[i]);
                    ItemIndex[srdItems[i].ID] = srdItems[i];
                    i++;
                }
            }
        }
    }

}
