using System.Collections.Generic;
using HB.Match3.Cell;

namespace HB.Match3.Board
{
    public class PossibleMatch
    {
        // Main cell to swap for match
        public global::MyCell mainCell;
        // swap direction to match
        public Direction direction;
        // other two cells that match will happen there
        public List<global::MyCell> otherCells;

        public override string ToString()
        {
            string otherCellPoses = "";
            foreach (var otherCell in otherCells)
            {
                otherCellPoses += otherCell.position + ",";
            }
            return $"{mainCell?.position} -> {direction} - ({otherCellPoses})";
        }
    }
}