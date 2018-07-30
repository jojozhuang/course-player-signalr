
namespace CoursePlayer.Core
{    
    public enum WBPenEvent
    {
        RedPenDown = -1,
        BluePenDown = -2,
        GreenPenDown = -3,
        BlackPenDown = -8,
        WideEraserDown = -9,
        NarrowEraserDown = -10,
        PenUp = -100,
        FunctionClear = -200
    }

    public enum WBPenColor
    {
        Red = -1,
        Blue = -2,
        Green = -3,
        Black = -8,
    }

    public enum WBEraserPenWidth
    {
        NarrowEraserWidth = 8 * 10 / 12,
        WideEraserWidth = 39 * 10 / 12
    }

    public enum PlayerState
    {
        Stopped = 0,
        Playing = 1,
        Paused = 2
    };
}
