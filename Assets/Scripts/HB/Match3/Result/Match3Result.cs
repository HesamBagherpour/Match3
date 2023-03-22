using System.Collections.Generic;

namespace HB.Match3.Result
{
    public class Match3Result
    {
        public WinStatus winStatus;
        public List<QuestData> RemainingQuests { get; set; }
        public int RemainingMoves { get; set; }
        public int TotalCoupons { get; set; }
    }
}