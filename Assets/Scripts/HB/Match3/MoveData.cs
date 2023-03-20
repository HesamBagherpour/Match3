using System.Collections.Generic;
using Garage.Match3;
using Garage.Match3.View.Quest;

namespace HB.Match3
{
    public class MoveData
    {
        public List<QuestData> RemainingQuests { get; set; }
        public List<MatchInfo> MatchInfos { get; set; }
        public int RemainingMoves { get; set; }
        public int TotalCoupons { get; set; }
    }
}