using System.Collections.Generic;
using Garage.Match3.View.Quest;

namespace HB.Match3
{
    public class Match3Result
    {
        public WinStatus winStatus;
        public List<QuestData> RemainingQuests { get; set; }
        public int RemainingMoves { get; set; }
        public int TotalCoupons { get; set; }
    }
}