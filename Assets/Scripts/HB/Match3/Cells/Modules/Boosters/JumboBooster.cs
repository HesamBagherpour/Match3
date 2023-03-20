using System;
using System.Collections.Generic;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.View;

namespace Garage.Match3.Cells.Modules.Boosters
{
    public class JumboBooster : Booster
    {
        private List<Point> cellPoints;

        public JumboBooster(Board board, Point pos, int damage) : base(board, pos, damage)
        {
            TrailID = string.Empty;
            cellPoints = new List<Point>();
        }

        public override void Activate()
        {
            if (cellPoints.Count == 0) cellPoints = GetCellPositions();
            UpdateAffectedParquets();
            MatchInfo matchInfo = new MatchInfo()
            {
                MatchedCells = new List<Cell>(),
                matchType = MatchType.Booster,
                OriginCell = board.Cells[pos.x, pos.y]
            };
            for (int i = 0; i < cellPoints.Count; i++)
            {
                Cell cell = board.Cells[cellPoints[i].x, cellPoints[i].y];
                HitCell(cell, BoosterType.Explosion, damage);
                matchInfo.MatchedCells.Add(cell);
            }
            board.MatchInfos.Add(matchInfo);
        }

        public override List<Point> GetCellPositions()
        {
            cellPoints = new List<Point>();
            for (int i = pos.x - 2; i <= pos.x + 2; i++) AddCellPoint(i, pos.y);
            for (int i = pos.y - 2; i <= pos.y + 2; i++) AddCellPoint(pos.x, i);
            AddCellPoint(pos.x - 1, pos.y - 1);
            AddCellPoint(pos.x - 1, pos.y + 1);
            AddCellPoint(pos.x + 1, pos.y - 1);
            AddCellPoint(pos.x + 1, pos.y + 1);
            return cellPoints;
        }

        private void AddCellPoint(int x, int y)
        {
            if (board.IsInBounds(x, y) && (x, y) != (pos.x, pos.y))
                cellPoints.Add(board.Cells[x, y].position);
        }

        public override BlockType GetBlockType()
        {
            return board.Cells[pos.x, pos.y].GetBlockType();
        }

        protected override void UpdateAffectedParquets()
        {
            bool hasParquet = false;
            foreach (var cellPoint in cellPoints)
            {
                if (board.Cells[cellPoint.x, cellPoint.y].CanDistributeparquet())
                {
                    hasParquet = true;
                    break;
                }
            }
            if (hasParquet)
            {
                BoardView.PlayAudio("parquet");
                foreach (var cellPoint in cellPoints)
                {
                    board.Cells[cellPoint.x, cellPoint.y].MarkForParquet();
                }
            }
        }
    }
}