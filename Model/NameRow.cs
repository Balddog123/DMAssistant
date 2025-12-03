using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public class NameRow
    {
        public enum NameComponent
        {
            Prefix,
            Core,
            Suffix
        }

        public string Male { get; set; }
        public string Female { get; set; }

        public NameRow(string male, string female)
        {
            Male = male;
            Female = female;
        }
    }
}
