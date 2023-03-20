using System;

namespace HB.Match3
{
    [Flags]
    public enum MatchType
    {
        None = 0,
        Horizontal = 1 << 2,
        Vertical = 1 << 3,
        Rainbow = 1 << 4,
        Cross = 1 << 5,
        Square = 1 << 6,
        Booster = 1 << 7,
        ExittingPlant = 1 << 8,
        Nalbaki = 1<<9,
        Flower = 1 << 9

    }
}