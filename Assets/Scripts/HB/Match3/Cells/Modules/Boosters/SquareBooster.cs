using System.Collections.Generic;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.View;

namespace Garage.Match3.Cells.Modules.Boosters
{
    public class SquareBooster : Booster
    {
        private List<Point> questPositions;

        public SquareBooster(Board board, Point pos, int damage) : base(board, pos, damage)
        {
            TrailID = "star-trail";
            questPositions = new List<Point>();
        }

        public override void Activate()
        {
            if (questPositions.Count == 0) questPositions = GetCellPositions();
            UpdateAffectedParquets();
            MatchInfo matchInfo = new MatchInfo()
            {
                MatchedCells = new System.Collections.Generic.List<Cell>(),
                matchType = MatchType.Booster,
                OriginCell = board.Cells[pos.x, pos.y]
            };
            for (int i = 0; i < questPositions.Count; i++)
            {
                Cell cell = board.Cells[questPositions[i].x, questPositions[i].y];
                HitCell(cell, BoosterType.Square, damage);
                matchInfo.MatchedCells.Add(cell);
            }
            board.MatchInfos.Add(matchInfo);
        }

        public override List<Point> GetCellPositions()
        {
            if (questPositions.Count == 0)
            {
                var q = QuestGiver.GetQuestPos();
                if (q != Point.None)
                {
                    questPositions = new List<Point>() { q };
                }
            }
            return questPositions;
        }
        public override BlockType GetBlockType()
        {
            return BlockType.None;
        }

        protected override void UpdateAffectedParquets()
        {
            if (board.Cells[pos.x, pos.y].CanDistributeparquet())
            {
                BoardView.PlayAudio("parquet");
                foreach (var cellPos in questPositions)
                {
                    board.Cells[cellPos.x, cellPos.y].MarkForParquet();
                }
            }
        }
    }
}