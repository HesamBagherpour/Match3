using System.Collections.Generic;

namespace HB.Match3.Models
{
    public class BoosterInfo
    {
        public BoosterType Type;
        public MyCell OriginCell;
        public List<MyCell> cells;
    }
}