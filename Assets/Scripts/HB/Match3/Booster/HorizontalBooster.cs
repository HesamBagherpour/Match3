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
    public class HorizontalBooster : MainBooster.Booster
    {
        public HorizontalBooster(Match3MainBoard.Board board, Point pos, int damage) : base(board, pos, damage)
        {
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
                HitCell(cell, BoosterType.Horizontal, damage);
                matchInfo.MatchedCells.Add(cell);
            }
            board.MatchInfos.Add(matchInfo);
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
                MyCell rightParquet = null;
                MyCell leftParquet = null;
                foreach (var cellPoint in cellPoints)
                {
                    MyCell cell = board.Cells[cellPoint.x, cellPoint.y];
                    if (cell.CanDistributeparquet())
                    {
                        if (cellPoint.x > pos.x)
                            rightParquet = cell;
                        else if (cellPoint.x < pos.x)
                            leftParquet = cell;
                    }
                }
                if (rightParquet != null)
                {
                    foreach (var cellpoint in cellPoints)
                    {
                        if (cellpoint.x > rightParquet.position.x) board.Cells[cellpoint.x, cellpoint.y].MarkForParquet();
                    }
                }
                if (leftParquet != null)
                {
                    foreach (var cellpoint in cellPoints)
                    {
                        if (cellpoint.x < leftParquet.position.x) board.Cells[cellpoint.x, cellpoint.y].MarkForParquet();
                    }
                }
                if (rightParquet != null || leftParquet != null)
                    BoardView.PlayAudio("parquet");
            }
        }

        public override BlockType GetBlockType()
        {
            return BlockType.None;
        }

        public override List<Point> GetCellPositions()
        {
            int y = pos.y;
            List<Point> points = new List<Point>();
            for (int x = 0; x < board.Width; x++)
            {
                points.Add(board.Cells[x, y].position);
            }
            return points;
        }
    }
}