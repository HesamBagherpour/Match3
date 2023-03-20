using System.Collections.Generic;
using HB.Match3;

namespace Garage.Match3.Cells.Modules.Boosters
{
    public class CrossBooster : Booster
    {
        private readonly HorizontalBooster _horizontal;
        private readonly VerticalBooster _vertical;

        public CrossBooster(Board board, Point pos, int damage) : base(board, pos, damage)
        {
            _horizontal = new HorizontalBooster(board, pos, damage);
            _vertical = new VerticalBooster(board, pos, damage);
        }

        public override void Activate()
        {
            _horizontal.Activate();
            _vertical.Activate();
        }

        public override BlockType GetBlockType()
        {
            return BlockType.None;
        }

        public override List<Point> GetCellPositions()
        {
            List<Point> points = new List<Point>();
            points.AddRange(_horizontal.GetCellPositions());
            points.AddRange(_vertical.GetCellPositions());
            return points;
        }

        protected override void UpdateAffectedParquets() { }
    }
}