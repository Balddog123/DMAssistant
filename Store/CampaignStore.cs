using CommunityToolkit.Mvvm.ComponentModel;
using DMAssistant.Helpers;
using DMAssistant.Model;
using DMAssistant.Repository;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
        public event Action<NPC> NPCDeleted;
        public Dictionary<string, Item> ItemIndex { get; private set; } = new Dictionary<string, Item>();
        public event Action<Item> ItemDeleted;
        public Dictionary<string, Location> LocationIndex { get; private set; } = new Dictionary<string, Location>();
        public event Action<Location> LocationDeleted;

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

            if (campaign.Monsters.Count < 100) campaign.SetMonsters(new ObservableCollection<Monster>(MonsterRepository.GetAllMonsters()));
            Debug.WriteLine(campaign.Monsters.Count);

            MonsterIndex = campaign.Monsters.ToDictionary(m => m.ID, m => m);
            NPCIndex = campaign.NPCs.ToDictionary(m => m.ID, m => m);
            ItemIndex = campaign.Items.ToDictionary(m => m.ID, m => m);
            LocationIndex = campaign.Locations.ToDictionary(m => m.ID, m => m);
            CurrentCampaign = campaign;
        }

        public void DeleteLocation(Location location, Session removeFromSession)
        {
            if (location == null) return;

            if (removeFromSession != null)
            {
                foreach (var session in CurrentCampaign.Sessions)
                {
                    if (removeFromSession == session && session.LocationIDs.Contains(location.ID)) session.LocationIDs.Remove(location.ID);
                }
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
            }            

            LocationDeleted?.Invoke(location);
        }
        public void DeleteNPC(NPC npc, Session removeFromSession)
        {
            if (npc == null) return;

            if (removeFromSession != null)
            {
                //remove just the reference from a single session
                foreach (var session in CurrentCampaign.Sessions)
                {
                    if (removeFromSession == session && session.NPCIDs.Contains(npc.ID)) session.NPCIDs.Remove(npc.ID);
                }
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
            }

            NPCDeleted?.Invoke(npc);
        }
        public void DeleteItem(Item item, Session removeFromSession)
        {
            if (item == null) return;

            if (removeFromSession != null)
            {
                //remove just the reference from a single session
                foreach (var session in CurrentCampaign.Sessions)
                {
                    if (removeFromSession == session && session.ItemIDs.Contains(item.ID)) session.ItemIDs.Remove(item.ID);
                }
            }
            else
            {
                //remove from EVERYTHING!

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
            }

            ItemDeleted?.Invoke(item);
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
    }

}
