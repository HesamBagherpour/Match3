using System.Collections.Generic;
using HB.Match3.Block;
using HB.Match3.Cell;
using HB.Match3.Models;
using HB.Match3.Modules;

namespace HB.Match3.Booster.MainBooster
{
    public abstract class Booster
    {
        protected Match3MainBoard.Board board;
        protected readonly Point pos;
        protected readonly int damage;
        public string TrailID { get; protected set; }

        protected Booster(Match3MainBoard.Board  board, Point pos, int damage)
        {
            this.board = board;
            this.pos = pos;
            this.damage = damage;
            TrailID = string.Empty;
        }

        public abstract void Activate();
        public abstract BlockType GetBlockType();
        public abstract List<Point> GetCellPositions();
        protected abstract void UpdateAffectedParquets();
        protected void HitCell(MyCell cell, BoosterType boosterType, int damage)
        {
            if (cell.HitType == HitType.None)
            {
                switch (boosterType)
                {
                    case BoosterType.Horizontal:
                    case BoosterType.Vertical:
                    case BoosterType.Cross:
                        var booster = cell.GetModule<BoosterModule>();
                        if (booster != null)
                        {
                            booster.UpdateBoosterType(boosterType, cell.position);
                        }
                        break;
                }
            }
            cell.Hit(HitType.Direct, damage);
        }
    }
}