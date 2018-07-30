using System;

namespace CoursePlayer.Core.Models
{
    public class Index : IComparable<Index>
    {
        public Index(ushort timestamp, byte grid, int offset, uint length)
        {
            TimeStamp = timestamp;
            Grid = grid;
            Offset = offset;
            DataLength = length;
        }

        public ushort TimeStamp { get; } //in minute for whiteboard, in second for screenshot
        public byte Grid { get; }
        public int Offset { get; set; }
        public uint DataLength { get; set; }

        public byte Row
        {
            get { return (byte)(Grid >> 4); }
        }
        public byte Col
        {
            get { return (byte)(Grid & 0xf); }
        }

        public static int StreamSize
        {
            get { return 2 + 1 + 4 + 4; }
        }

        public int CompareTo(Index obj)
        {
            int compare = TimeStamp.CompareTo(obj.TimeStamp);
            if (compare == 0)
            {
                compare = Grid.CompareTo(obj.Grid);
            }
            return compare;
        }
    }
}
