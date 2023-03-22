using System.Collections.Generic;
using HB.Match3.Block;
using HB.Match3.Board;
using HB.Match3.Board.BoardStates;
using HB.Match3.Cell;
using HB.Match3.Match;
using HB.Match3.Models;
using HB.Match3.Modules;
using HB.Packages.Utilities;

namespace HB.Match3.Booster
{
    public class RainbowBooster : MainBooster.Booster
    {
     private readonly List<MyCell> _cells;
        private BlockType _blockType;

        public RainbowBooster(Match3MainBoard.Board board, Point pos, int damage) : base(board, pos, damage)
        {
            _cells = new List<MyCell>(10);
            TrailID = "trail";
        }

        public override void Activate()
        {
            GetCells();
            UpdateAffectedParquets();

            if (_blockType != BlockType.None)
            {
                MatchInfo matchInfo = new MatchInfo()
                {
                    MatchedCells = new System.Collections.Generic.List<MyCell>(),
                    matchType = MatchType.Booster,
                    OriginCell = board.Cells[pos.x, pos.y]
                };
                for (int i = 0; i < _cells.Count; i++)
                {
                    MyCell cell = _cells[i];
                    HitCell(cell, BoosterType.Rainbow, damage);
                    matchInfo.MatchedCells.Add(cell);
                }
                board.MatchInfos.Add(matchInfo);
            }
        }

        private void GetCells()
        {
            MyCell originCell = board.Cells[pos.x, pos.y];
            BlockType bt = originCell.GetPreviousBlockType();
            if (bt.id.Contains(BlockIDs.Box) == false)
            {
                bt = originCell.GetBlockType();
                if (bt == BlockType.None)
                {
                    bt = originCell.GetPreviousBlockType();
                    if (bt == BlockType.None)
                        bt = board.RandomBlockType;
                }
            }
            GetCellsByBlockType(bt);
            _blockType = bt;
        }

        public override List<Point> GetCellPositions()
        {
            List<Point> effectedPositions = new List<Point>();
            for (int i = 0; i < _cells.Count; i++)
            {
                effectedPositions.Add(_cells[i].position);
            }

            return effectedPositions;
        }

        private void GetCellsByBlockType(BlockType bt)
        {
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    MyCell cell = board.Cells[x, y];
                    BlockModule block = cell.GetModule<BlockModule>();
                    if (block != null)
                    {
                        if (bt.id.Contains(BlockIDs.Box))
                        {
                            if (block.blockType.color == bt.color)
                            {
                                _cells.Add(cell);
                            }
                        }
                        else
                        {
                            if (block.blockType == bt)
                            {
                                _cells.Add(cell);
                            }
                        }
                    }
                }
            }
        }

        private void GetRandomBlocks()
        {
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    MyCell cell = board.Cells[x, y];
                    if (cell.IsVisible)
                    {
                        _cells.Add(cell);
                    }
                }
            }

            _cells.Shuffle();

            if (_cells.Count > 10)
            {
                _cells.RemoveRange(9, _cells.Count - 9);
            }
        }

        public override BlockType GetBlockType()
        {
            return _blockType;
        }

        protected override void UpdateAffectedParquets()
        {
            if (board.Cells[pos.x, pos.y].CanDistributeparquet())
            {
                BoardView.PlayAudio("parquet");
                foreach (var cell in _cells)
                {
                    cell.MarkForParquet();
                }
            }
        }
    }
}