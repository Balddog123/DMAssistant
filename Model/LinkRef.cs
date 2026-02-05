using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public enum LinkType
    {
        None,
        Spell,
        NPC,
        Item,
        Location,
        Monster
    }

    public sealed class LinkRef
    {
        public LinkType Type { get; }
        public string Key { get; }

        public LinkRef(LinkType type, string id)
        {
            Type = type;
            Key = id;
        }

        public override string ToString() => $"{Type}:{Key}";
    }
}
