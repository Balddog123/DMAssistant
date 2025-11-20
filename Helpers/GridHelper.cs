using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMAssistant.Helpers
{
    public static class GridHelper
    {
        public static List<LineInfo> GetGridLines()
        {
            const int spacing = 50;
            const int size = 5000;

            var lines = new List<LineInfo>();

            for (int x = 0; x < size; x += spacing)
            {
                lines.Add(new LineInfo { X1 = x, Y1 = 0, X2 = x, Y2 = size });
            }
            for (int y = 0; y < size; y += spacing)
            {
                lines.Add(new LineInfo { X1 = 0, Y1 = y, X2 = size, Y2 = y });
            }

            return lines;
        }
    }

    public class LineInfo
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
    }

}
