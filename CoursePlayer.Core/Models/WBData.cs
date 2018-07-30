using System.Collections.Generic;

namespace CoursePlayer.Core.Models
{
    public class WBData
    {
        public WBData(int second, List<WBLine> lines, List<WBEvent> events)
        {
            Second = second;
            WBLines = lines;
            WBEvents = events;
        }

        public int Second { get; set; }
        public List<WBLine> WBLines { get; set; }
        public List<WBEvent> WBEvents { get; set; }
    }
}
