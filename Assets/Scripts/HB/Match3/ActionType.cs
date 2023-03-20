using System;

namespace Garage.Match3
{
    [Flags]
    public enum ActionType
    {
        Swap = 1 << 1,
        Move = 1 << 2,
        Match = 1 << 3,
        Collect = 1 << 4,
        Spawn = 1 << 5,
        HitBlock = 1 << 6,
        BlockCannon = 1 << 7,
    }
}