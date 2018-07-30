namespace CoursePlayer.Core.Models
{
    public class WBEvent
    {
        public WBEvent(uint timestamp, ushort reserved, int x, int y)
        {
            TimeStamp = timestamp;
            Reserved = reserved; // 0 by default
            X = x;
            Y = y;
        }

        public uint TimeStamp { get; set; } // From 0 to 60000, millseconds(0~59)
        public ushort Reserved { get; set; }
        public int X { get; set; } // It is special event based on its value
        public int Y { get; set; }

        public static int StreamSize
        {
            get { return 2 * 4; }
        }
    }
}
