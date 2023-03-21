using HB.Match3.Cell;
using System;
using System.Collections.Generic;

namespace HB.Match3.Behaviours
{
    public class PossibleMatch
    {
        // Main cell to swap for match
        public MyCell mainCell;
        // swap direction to match
        public Direction direction;
        // other two cells that match will happen there
        public List<MyCell> otherCells;

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