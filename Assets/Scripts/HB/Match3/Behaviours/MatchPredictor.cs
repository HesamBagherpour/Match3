using System;
using System.Collections.Generic;
using HB.Match3.Block;
using HB.Match3.Cell;
using HB.Match3.Modules;

namespace HB.Match3.Behaviours
{
    public class MatchPredictor
    {
        #region Private Fields

        private Match3MainBoard.Board _board;
        private List<MyCell> _otherCellsInPossibleMatch;
        #endregion

        #region  Constructors

     
        public MatchPredictor(Match3MainBoard.Board board)
        {
            _board = board;
            _otherCellsInPossibleMatch = new List<MyCell>();
        }
   

        #endregion

        #region Public Methods

        public bool WillMatchBySwap(MyCell cell, MyCell otherCell)
        {
            _otherCellsInPossibleMatch.Clear();
            if (cell == otherCell) return false;
            if (cell.IsVisible == false || otherCell.IsVisible == false) return false;
            if (cell.IsLocked(ActionType.Swap, Direction.All) || otherCell.IsLocked(ActionType.Swap, Direction.All)) return false;
            if (WallBetweenCells(cell, otherCell)) return false;

            BlockModule block = cell.GetModule<BlockModule>();
            if (block == null) return false;
            BlockType bt = block.blockType;
            int ox = otherCell.position.x;
            int oy = otherCell.position.y;

            cell.ExchangeBlocks(otherCell);

            bool ret = WillMatchHorizontal(ox, oy, bt) ||
                       WillMatchVertical(ox, oy, bt) ||
                       WillMatchSqure(ox, oy, bt);

            cell.ExchangeBlocks(otherCell);
            SetPossibleMatch(cell, otherCell, ret);
            return ret;
        }

        private bool WallBetweenCells(MyCell cell, MyCell otherCell)
        {
            // (1,2) = > (0,2)
            if (cell.position.x == 1 && cell.position.y == 2 && otherCell.position.x == 0 && otherCell.position.y == 2)
            {
                var ps = cell.position;
                ps.x++;
            }
            IronWallModule cellWall = cell.GetModule<IronWallModule>();
            IronWallModule otherCellWall = otherCell.GetModule<IronWallModule>();
            if (cellWall == null && otherCellWall == null) return false;
            // cells are horizontal neighbours
            if (cell.position.x + 1 == otherCell.position.x)
            {
                // other cell is at the right direction
                if (cellWall != null && cellWall.Restriction.Contains(ActionType.Swap, Direction.Right)) return true;
                if (otherCellWall != null && otherCellWall.Restriction.Contains(ActionType.Swap, Direction.Left)) return true;
            }
            else if (cell.position.x - 1 == otherCell.position.x)
            {
                // other cell is at the left
                if (cellWall != null && cellWall.Restriction.Contains(ActionType.Swap, Direction.Left)) return true;
                if (otherCellWall != null && otherCellWall.Restriction.Contains(ActionType.Swap, Direction.Right)) return true;
            }
            else if (cell.position.y + 1 == otherCell.position.y)
            {
                // other cell is at the top direction
                if (cellWall != null && cellWall.Restriction.Contains(ActionType.Swap, Direction.Top)) return true;
                if (otherCellWall != null && otherCellWall.Restriction.Contains(ActionType.Swap, Direction.Bottom)) return true;
            }
            else if (cell.position.y - 1 == otherCell.position.y)
            {
                // other cell is at the bottom
                if (cellWall != null && cellWall.Restriction.Contains(ActionType.Swap, Direction.Bottom)) return true;
                if (otherCellWall != null && otherCellWall.Restriction.Contains(ActionType.Swap, Direction.Top)) return true;
            }
            return false;
        }

        private void SetPossibleMatch(MyCell cell, MyCell otherCell, bool ret)
        {
            if (ret)
            {
                if (_otherCellsInPossibleMatch.Contains(otherCell)) _otherCellsInPossibleMatch.Remove(otherCell);
                if (_otherCellsInPossibleMatch.Contains(cell)) _otherCellsInPossibleMatch.Remove(cell);
                Direction direction = Direction.None;
                if (cell.position.x + 1 == otherCell.position.x) direction = Direction.Right;
                else if (cell.position.x - 1 == otherCell.position.x) direction = Direction.Left;
                else if (cell.position.y + 1 == otherCell.position.y) direction = Direction.Top;
                else if (cell.position.y - 1 == otherCell.position.y) direction = Direction.Bottom;
                //HB
                // if (direction != Direction.None) _board.PossibleMatch = new PossibleMatch()
                // {
                //     mainCell = cell,
                //     direction = direction,
                //     otherCells = _otherCellsInPossibleMatch
                // };
                //HB
            }
        }


        public bool IsMatch(MyCell cell)
        {
            int ox = cell.position.x;
            int oy = cell.position.y;

            BlockType bt = GetBlockType(ox, oy);
            if (bt == BlockType.None) return false;

            return WillMatchHorizontal(ox, oy, bt) ||
                   WillMatchVertical(ox, oy, bt)
                   || WillMatchSqure(ox, oy, bt);
        }

        #endregion

        #region Private Methods

        private bool WillMatchVertical(int ox, int oy, BlockType bt)
        {
            return WillMatchVerticalMid(ox, oy, bt) ||
                   WillMatchToBottom(ox, oy, bt) ||
                   WillMatchToTop(ox, oy, bt);
        }

        private bool WillMatchToTop(int ox, int oy, BlockType bt)
        {
            int up = Top(oy, 2);
            int down = Down(up, 2);

            int matchCount = 0;
            for (int y = down; y <= up; y++)
            {
                BlockType _bt = GetBlockType(ox, y);
                if (_bt.CanMatchWith(bt) == false) continue;
                matchCount++;
                if (matchCount == 3)
                {
                    for (int y1 = down; y1 <= up; y1++)
                    {
                        _otherCellsInPossibleMatch.Add(_board.Cells[ox, y1]);
                    }
                    return true;
                }
            }

            return false;
        }

        private bool WillMatchToBottom(int ox, int oy, BlockType bt)
        {
            int down = Down(oy, 2);
            int up = Top(down, 2);

            int matchCount = 0;
            for (int y = down; y <= up; y++)
            {
                BlockType _bt = GetBlockType(ox, y);
                if (_bt.CanMatchWith(bt) == false) continue;
                matchCount++;
                if (matchCount == 3)
                {
                    for (int y1 = down; y1 <= up; y1++)
                    {
                        _otherCellsInPossibleMatch.Add(_board.Cells[ox, y1]);
                    }
                    return true;
                }
            }

            return false;
        }

        private bool WillMatchVerticalMid(int ox, int oy, BlockType bt)
        {
            int down = Down(oy, 1);
            int up = Top(down, 2);

            int matchCount = 0;
            for (int y = down; y <= up; y++)
            {
                BlockType _bt = GetBlockType(ox, y);
                if (_bt.CanMatchWith(bt) == false) continue;
                matchCount++;
                if (matchCount == 3)
                {
                    for (int y1 = down; y1 <= up; y1++)
                    {
                        _otherCellsInPossibleMatch.Add(_board.Cells[ox, y1]);
                    }
                    return true;
                }
            }

            return false;
        }

        private bool WillMatchHorizontal(int ox, int oy, BlockType bt)
        {
            return WillMatchHorizontalMid(ox, oy, bt) ||
                   WillMatchToLeft(ox, oy, bt) ||
                   WillMatchToRight(ox, oy, bt);
        }

        private bool WillMatchHorizontalMid(int ox, int oy, BlockType bt)
        {
            int left = Left(ox, 1);
            int right = Right(left, 2);
            int matchCount = 0;
            for (int x = left; x <= right; x++)
            {
                BlockType _bt = GetBlockType(x, oy);
                if (_bt.CanMatchWith(bt) == false) continue;
                matchCount++;
                if (matchCount == 3)
                {
                    for (int x1 = left; x1 <= right; x1++)
                    {
                        _otherCellsInPossibleMatch.Add(_board.Cells[x1, oy]);
                    }
                    return true;
                }
            }

            return false;
        }

        private BlockType GetBlockType(int x, int y)
        {
            MyCell cell = _board.Cells[x, y];
            if (cell.IsLocked(ActionType.Match, Direction.Center) ||
                cell.IsRestrictedBlock(ActionType.Match) ||
                cell.Contains<CannonModule>())
                return BlockType.None;

            return cell.GetBlockType();
        }


        private bool WillMatchToLeft(int ox, int oy, BlockType bt)
        {
            int left = Left(ox, 2);
            int right = Right(left, 2);
            int matchCount = 0;
            for (int x = left; x <= right; x++)
            {
                BlockType _bt = GetBlockType(x, oy);
                if (_bt.CanMatchWith(bt) == false) continue;
                matchCount++;
                if (matchCount == 3)
                {
                    for (int x1 = left; x1 <= right; x1++)
                    {
                        _otherCellsInPossibleMatch.Add(_board.Cells[x1, oy]);
                    }
                    return true;
                }
            }

            return false;
        }

        private bool WillMatchToRight(int ox, int oy, BlockType bt)
        {
            int right = Right(ox, 2);
            int left = Left(right, 2);
            int matchCount = 0;

            for (int x = left; x <= right; x++)
            {
                BlockType _bt = GetBlockType(x, oy);
                if (_bt.CanMatchWith(bt) == false) continue;
                matchCount++;
                if (matchCount == 3)
                {
                    for (int x1 = left; x1 <= right; x1++)
                    {
                        _otherCellsInPossibleMatch.Add(_board.Cells[x1, oy]);
                    }
                    return true;
                }
            }

            return false;
        }

        private bool WillMatchSqure(int ox, int oy, BlockType bt)
        {
            return WillMatchSqureBottomLeft(ox, oy, bt) ||
                   WillMatchSqureBottomRight(ox, oy, bt) ||
                   WillMatchSqureTopLeft(ox, oy, bt) ||
                   WillMatchSqureTopRight(ox, oy, bt);
        }

        private bool WillMatchSqureBottomLeft(int ox, int oy, BlockType bt)
        {
            int left = Left(ox, 1);
            if (left == ox) return false;
            int down = Down(oy, 1);
            if (down == oy) return false;

            BlockType block1 = GetBlockType(left, down);
            BlockType block2 = GetBlockType(left, oy);
            BlockType block3 = GetBlockType(ox, down);
            return AreMatch(bt, block1, block2, block3);
        }

        private bool WillMatchSqureBottomRight(int ox, int oy, BlockType bt)
        {
            int right = Right(ox, 1);
            if (right == ox) return false;
            int down = Down(oy, 1);
            if (down == oy) return false;

            BlockType block1 = GetBlockType(right, down);
            BlockType block2 = GetBlockType(right, oy);
            BlockType block3 = GetBlockType(ox, down);
            return AreMatch(bt, block1, block2, block3);
        }

        private bool WillMatchSqureTopLeft(int ox, int oy, BlockType bt)
        {
            int top = Top(oy, 1);
            if (top == oy) return false;
            int left = Left(ox, 1);
            if (left == ox) return false;
            BlockType block1 = GetBlockType(left, top);
            BlockType block2 = GetBlockType(left, oy);
            BlockType block3 = GetBlockType(ox, top);
            return AreMatch(bt, block1, block2, block3);
        }

        private bool WillMatchSqureTopRight(int ox, int oy, BlockType bt)
        {
            int top = Top(oy, 1);
            if (top == oy) return false;
            int right = Right(ox, 1);
            if (right == ox) return false;
            BlockType block1 = GetBlockType(right, top);
            BlockType block2 = GetBlockType(right, oy);
            BlockType block3 = GetBlockType(ox, top);
            return AreMatch(bt, block1, block2, block3);
        }

        private static bool AreMatch(BlockType bt, BlockType block1, BlockType block2, BlockType block3)
        {
            return block1.CanMatchWith(bt) &&
                   block2.CanMatchWith(bt) &&
                   block3.CanMatchWith(bt);
        }

        private int Top(int down, int offest)
        {
            return Math.Min(down + offest, _board.Height - 1);
        }

        private int Down(int oy, int offset)
        {
            return Math.Max(0, oy - offset);
        }

        private int Right(int left, int offset)
        {
            return Math.Min(_board.Width - 1, left + offset);
        }

        private int Left(int ox, int offset)
        {
            return Math.Max(0, ox - offset);
        }

        #endregion
    }
}