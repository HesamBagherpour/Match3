using System.Collections.Generic;
using HB.Match3.Block;
using HB.Match3.Board;
using HB.Match3.Board.BoardStates;
using HB.Match3.Cell;
using HB.Match3.Match;
using HB.Match3.Models;
using HB.Match3.Modules;

namespace HB.Match3.Booster
{
    public class VerticalBooster : MainBooster.Booster
    {
        public VerticalBooster(Match3MainBoard.Board board, Point pos, int damage) : base(board, pos, damage)
        {
            TrailID = "";
        }
        private List<Point> cellPoints;

        public override void Activate()
        {
            if (cellPoints == null || cellPoints.Count == 0) cellPoints = GetCellPositions();
            UpdateAffectedParquets();
            MatchInfo matchInfo = new MatchInfo()
            {
                MatchedCells = new List<MyCell>(),
                matchType = MatchType.Booster,
                OriginCell = board.Cells[pos.x, pos.y]
            };
            for (int i = 0; i < cellPoints.Count; i++)
            {
                MyCell cell = board.Cells[cellPoints[i].x, cellPoints[i].y];
                HitCell(cell, BoosterType.Vertical, damage);
                matchInfo.MatchedCells.Add(cell);
            }
            board.MatchInfos.Add(matchInfo);
        }
        public override BlockType GetBlockType()
        {
            return BlockType.None;
        }

        public override List<Point> GetCellPositions()
        {
            int x = pos.x;
            List<Point> points = new List<Point>();
            for (int y = 0; y < board.Height; y++)
            {
                points.Add(board.Cells[x, y].position);
            }
            return points;
        }

        protected override void UpdateAffectedParquets()
        {
            // if pos contains parquet, parquet all cellpoints
            if (board.Cells[pos.x, pos.y].CanDistributeparquet())
            {
                BoardView.PlayAudio("parquet");
                foreach (var cellPoint in cellPoints)
                {
                    MyCell cell = board.Cells[cellPoint.x, cellPoint.y];
                    cell.MarkForParquet();
                }
            }
            else
            {
                MyCell topParquet = null;
                MyCell bottomParquet = null;
                foreach (var cellPoint in cellPoints)
                {
                    MyCell cell = board.Cells[cellPoint.x, cellPoint.y];
                    if (cell.CanDistributeparquet())
                    {
                        if (cellPoint.y > pos.y)
                            topParquet = cell;
                        else if (cellPoint.y < pos.y)
                            bottomParquet = cell;
                    }
                }
                if (topParquet != null)
                {
                    foreach (var cellpoint in cellPoints)
                    {
                        if (cellpoint.y > topParquet.position.y) board.Cells[cellpoint.x, cellpoint.y].MarkForParquet();
                    }
                }
                if (bottomParquet != null)
                {
                    foreach (var cellpoint in cellPoints)
                    {
                        if (cellpoint.y < bottomParquet.position.y) board.Cells[cellpoint.x, cellpoint.y].MarkForParquet();
                    }
                }
                if (topParquet != null || bottomParquet != null)
                    BoardView.PlayAudio("parquet");
            }
        }
    }
}