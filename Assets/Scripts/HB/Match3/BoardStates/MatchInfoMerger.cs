using System.Collections.Generic;
using Garage.Match3.Cells;
using HB.Match3;

namespace Garage.Match3.BoardStates
{
    public static class MatchInfoMerger
    {
        public static void Merge(this List<MatchInfo> matchInfos)
        {
            ConvertToRainbow(matchInfos);
            MergeIntersectingsWithRainbow(matchInfos);

            ConvertToCross(matchInfos);
            MergeIntersectingsWithCross(matchInfos);

            MergeIntersectingsWithBoosterLiniar(matchInfos);
            MergeIntersectingsWithSquare(matchInfos);
            // Log.Debug("MergeMatchInfos", "Complete");
        }

        private static void MergeIntersectingsWithSquare(List<MatchInfo> matchInfos)
        {
            var removeableMatchInfos = new List<MatchInfo>();
            for (int i = 0; i < matchInfos.Count; i++)
            {
                var matchInfo = matchInfos[i];
                if (removeableMatchInfos.Contains(matchInfo)) continue;
                if (matchInfo.matchType == MatchType.Square)
                {
                    for (int j = 0; j < matchInfos.Count; j++)
                    {
                        MatchInfo otherMatchInfo = matchInfos[j];
                        if (otherMatchInfo.MatchedCells.Count == 3)
                        {
                            Cell intersectCell = HasIntersection(matchInfo, otherMatchInfo);
                            if (intersectCell != null)
                            {
                                MergeMatchInfos(matchInfo, otherMatchInfo);
                                matchInfo.OriginCell = intersectCell;
                                removeableMatchInfos.Add(otherMatchInfo);
                            }
                        }
                    }
                }
            }
            foreach (var matchInfo in removeableMatchInfos)
            {
                matchInfos.Remove(matchInfo);
            }
        }
        private static void MergeIntersectingsWithBoosterLiniar(List<MatchInfo> matchInfos)
        {
            var removeableMatchInfos = new List<MatchInfo>();
            for (int i = 0; i < matchInfos.Count; i++)
            {
                var matchInfo = matchInfos[i];
                if (removeableMatchInfos.Contains(matchInfo)) continue;
                if ((matchInfo.matchType == MatchType.Horizontal ||
                    matchInfo.matchType == MatchType.Vertical) &&
                    matchInfo.MatchedCells.Count > 3)
                {
                    for (int j = 0; j < matchInfos.Count; j++)
                    {
                        MatchInfo otherMatchInfo = matchInfos[j];
                        Cell intersectCell = HasIntersection(matchInfo, otherMatchInfo);
                        if (intersectCell != null)
                        {
                            MergeMatchInfos(matchInfo, otherMatchInfo);
                            matchInfo.OriginCell = intersectCell;
                            removeableMatchInfos.Add(otherMatchInfo);
                        }
                    }
                }
            }
            foreach (var matchInfo in removeableMatchInfos)
            {
                matchInfos.Remove(matchInfo);
            }
        }
        private static void MergeIntersectingsWithCross(List<MatchInfo> matchInfos)
        {
            var removeableMatchInfos = new List<MatchInfo>();
            for (int i = 0; i < matchInfos.Count; i++)
            {
                var matchInfo = matchInfos[i];
                if (removeableMatchInfos.Contains(matchInfo)) continue;
                if (matchInfo.matchType == MatchType.Cross)
                {
                    for (int j = 0; j < matchInfos.Count; j++)
                    {
                        MatchInfo otherMatchInfo = matchInfos[j];
                        Cell intersectCell = HasIntersection(matchInfo, otherMatchInfo);
                        if (intersectCell != null)
                        {
                            MergeMatchInfos(matchInfo, otherMatchInfo);
                            matchInfo.matchType = MatchType.Cross;
                            removeableMatchInfos.Add(otherMatchInfo);
                        }
                    }
                }
            }
            foreach (var matchInfo in removeableMatchInfos)
            {
                matchInfos.Remove(matchInfo);
            }
        }
        private static void ConvertToCross(List<MatchInfo> matchInfos)
        {
            var removeableMatchInfos = new List<MatchInfo>();
            for (int i = 0; i < matchInfos.Count; i++)
            {
                var matchInfo = matchInfos[i];
                if (removeableMatchInfos.Contains(matchInfo)) continue;
                if (matchInfo.matchType == MatchType.Horizontal || matchInfo.matchType == MatchType.Vertical)
                {
                    for (int j = 0; j < matchInfos.Count; j++)
                    {
                        MatchInfo otherMatchInfo = matchInfos[j];
                        if (otherMatchInfo.matchType == MatchType.Horizontal ||
                            otherMatchInfo.matchType == MatchType.Vertical)
                        {
                            Cell intersectCell = HasIntersection(matchInfo, otherMatchInfo);
                            if (intersectCell != null)
                            {
                                MergeMatchInfos(matchInfo, otherMatchInfo);
                                matchInfo.matchType = MatchType.Cross;
                                matchInfo.OriginCell = intersectCell;
                                removeableMatchInfos.Add(otherMatchInfo);
                            }
                        }
                    }
                }
            }
            foreach (var matchInfo in removeableMatchInfos)
            {
                matchInfos.Remove(matchInfo);
            }
        }
        private static void MergeMatchInfos(MatchInfo matchInfo, MatchInfo otherMatchInfo)
        {
            for (int i = 0; i < otherMatchInfo.MatchedCells.Count; i++)
            {
                Cell cell = otherMatchInfo.MatchedCells[i];
                if (matchInfo.MatchedCells.Contains(cell) == false)
                {
                    matchInfo.MatchedCells.Add(cell);
                }
            }
        }
        private static Cell HasIntersection(MatchInfo matchInfo, MatchInfo otherMatchInfo)
        {
            if (matchInfo == otherMatchInfo) return null;
            foreach (var cell in matchInfo.MatchedCells)
            {
                foreach (var otherCell in otherMatchInfo.MatchedCells)
                {
                    if (cell == otherCell)
                    {
                        return cell;
                    }
                }
            }
            return null;
        }
        private static void MergeIntersectingsWithRainbow(List<MatchInfo> matchInfos)
        {
            List<MatchInfo> removableMatchInfos = new List<MatchInfo>();
            for (int i = 0; i < matchInfos.Count; i++)
            {
                var matchInfo = matchInfos[i];
                if (removableMatchInfos.Contains(matchInfo)) continue;
                if (matchInfo.matchType == MatchType.Rainbow)
                {
                    foreach (MatchInfo otherMatchInfo in matchInfos)
                    {
                        Cell intersectCell = HasIntersection(matchInfo, otherMatchInfo);
                        if (intersectCell != null)
                        {
                            MergeMatchInfos(matchInfo, otherMatchInfo);
                            removableMatchInfos.Add(otherMatchInfo);
                        }
                    }
                }
            }
            foreach (var matchInfo in removableMatchInfos)
            {
                matchInfos.Remove(matchInfo);
            }
        }
        private static void ConvertToRainbow(List<MatchInfo> matchInfos)
        {
            for (int i = 0; i < matchInfos.Count; i++)
            {
                var matchInfo = matchInfos[i];
                if ((matchInfo.matchType == MatchType.Horizontal || matchInfo.matchType == MatchType.Vertical) &&
                     matchInfo.MatchedCells.Count >= 5)
                {
                    matchInfo.matchType = MatchType.Rainbow;
                    matchInfo.OriginCell = matchInfo.MatchedCells[matchInfo.MatchedCells.Count / 2];
                }
            }
        }
    }
}