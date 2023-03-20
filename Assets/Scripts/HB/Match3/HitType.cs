using System;

namespace HB.Match3
{
    [Flags]
    public enum HitType
    {
        None = 0,
        Indirect = 1 << 1,
        Direct = 1 << 2,
    }
}