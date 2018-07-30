namespace CoursePlayer.Core.Models
{
    public class WBLine
    {
        public WBLine(ushort x0, ushort y0, ushort x1, ushort y1, short color, ushort reserved)
        {
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
            Color = color;
            Reserved = reserved;
        }

        public ushort X0 { get; set; }
        public ushort Y0 { get; set; }
        public ushort X1 { get; set; }
        public ushort Y1 { get; set; }
        public short Color { get; set; }
        public ushort Reserved { get; set; }

        public static int StreamSize
        {
            get { return 2 * 6; }
        }
    }
}
