using System.Collections.Generic;
using HB.Match3.Match;

namespace HB.Match3.Models
{
    public class MoveData
    {
        public List<Result.QuestData> RemainingQuests { get; set; }
        public List<MatchInfo> MatchInfos { get; set; }
        public int RemainingMoves { get; set; }
        public int TotalCoupons { get; set; }
    }
}