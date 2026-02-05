using DMAssistant.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DMAssistant.Helpers
{
    public static class HyperlinkHelper
    {
        public static object GetObject(LinkType type, string key)
        {
            Object objectForLink = null;
            if (type == LinkType.Spell)
            {
                var spellDict = App.CampaignStore.CurrentCampaign.Spells.ToDictionary(s => s.Name.ToLower().Trim(), s => s);
                if (spellDict.TryGetValue(key.ToLower().Trim(), out var spell))
                {
                    objectForLink = spell;
                }
                else return null;
            }
            else if (type == LinkType.Item)
            {
                var itemDict = App.CampaignStore.CurrentCampaign.Items.ToDictionary(i => i.Name.ToLower().Trim(), i => i);
                if (itemDict.TryGetValue(key.ToLower().Trim(), out var item))
                {
                    objectForLink = item;
                }
                else return null;
            }
            else if (type == LinkType.Monster)
            {
                var monsterDict = App.CampaignStore.CurrentCampaign.Monsters.ToDictionary(m => m.Name.ToLower().Trim(), m => m);
                if (monsterDict.TryGetValue(key.ToLower().Trim(), out var monster))
                {
                    objectForLink = monster;
                }
                else return null;
            }
            else if (type == LinkType.Location)
            {
                var locationDict = App.CampaignStore.CurrentCampaign.Locations.ToDictionary(l => l.Name.ToLower().Trim(), l => l);
                if (locationDict.TryGetValue(key.ToLower().Trim(), out var location))
                {
                    objectForLink = location;
                }
                else return null;
            }
            else if (type == LinkType.NPC)
            {
                var dict = App.CampaignStore.CurrentCampaign.NPCs.ToDictionary(n => n.Name.ToLower().Trim(), n => n);
                if (dict.TryGetValue(key.ToLower().Trim(), out var npc))
                {
                    objectForLink = npc;
                }
                else return null;
            }
            else return null;

            return objectForLink;
        }
    }
}
