using System.Collections.Generic;
using Garage.Match3.Cells;

namespace HB.Match3
{
    public class MatchInfo
    {
        #region Public Fields

        public List<Cell> MatchedCells;
        public Cell OriginCell;
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