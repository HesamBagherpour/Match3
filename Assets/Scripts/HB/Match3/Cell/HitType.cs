using System;
namespace HB.Match3.Cell
{
    [Flags]
    public enum HitType
    {
        None = 0,
        Indirect = 1 << 1,
        Direct = 1 << 2,
    }
}