using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Model
{
    public class NoteBoxData
    {
        public double X { get; set; }      // Canvas.Left
        public double Y { get; set; }      // Canvas.Top
        public double Width { get; set; }
        public double Height { get; set; }
        public string Text { get; set; } = "";
        public string Title { get; set; } = "";
    }
}
