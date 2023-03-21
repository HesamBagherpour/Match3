using System;

namespace HB.Match3.Cell
{
    [Flags]
    public enum Direction
    {
        None = 0,
        Top = 1,
        Left = 1 << 2,
        Bottom = 1 << 3,
        Right = 1 << 4,
        Center = 1 << 5,
        All = Top | Left | Bottom | Right | Center
    }
}