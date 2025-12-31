using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public partial class EncounterEvent : EncounterItem
    {
        public EncounterEvent()
        {
            name = "New Encounter Event";
        }

        protected EncounterEvent(EncounterEvent other) : base(other)
        {
            //for use later
        }

        public override EncounterItem Clone()
        {
            return new EncounterEvent(this);
        }
    }
}
