using System.Collections.Generic;
using HB.Match3.Models;

namespace HB.Match3.Match
{
    public class MatchInfo
    {
        #region Public Fields

        public List<MyCell> MatchedCells;
        public MyCell OriginCell;
        public MatchType matchType;
        #endregion

        #region Public Methods

        public override string ToString()
        {
            string str = $"MatchInfo: {matchType} {OriginCell} -> ";
            for (int i = 0; i < MatchedCells.Count; i++)
                str += MatchedCells[i] + ", ";

            return str;
        }

        #endregion
    }
}