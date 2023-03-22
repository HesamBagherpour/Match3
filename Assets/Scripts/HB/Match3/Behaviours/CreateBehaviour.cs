using HB.Match3.Block;
using HB.Match3.Board;

using System;
using System.Collections.Generic;
using System.Linq;
using HB.Match3.Cell;
using HB.Match3.Models;
using HB.Match3.Modules;
using HB.Packages.Logger;

namespace HB.Match3.Behaviours
{
    public class CreateBehaviour
    {
        public static class Setting
        {
            public const int MinInitCoupons = 2;
            public const int MaxInitCoupons = 4;
            public const int FallCouponChancePercent = 7;
        }
        #region Private Fields

        private readonly Match3MainBoard.Board _board;
        private readonly BoardData _boardData;
        private readonly IBoardView _boardView;
        private readonly BlockFactory _blockFactory;
        private readonly Random _random;
        private Action onCreateBoardFinished;
        private BlockType[] allBlockTypes = new BlockType[0];

        #endregion

        #region  Constructors

        public CreateBehaviour(Match3MainBoard.Board board, IBoardView boardView, BlockFactory blockFactory, BoardData data, Random random)
        {
            _board = board;
            _boardData = data;
            _boardView = boardView;
            _blockFactory = blockFactory;
            _random = random;
        }

        #endregion

        #region Public Methods

        public void CreateBoard(Action onFinished)
        {
            onCreateBoardFinished = onFinished;
            _boardView.SetBoardData(_boardData);
            _boardView.Hide();
            _board.CreateSofaViews();
            CreateInitialBlocks();
            CreateBlocksForEmptyCells();
            SetAdjacent();
            SetIronWallNeighbours();
            CreateLockCellQuests();
                //HB //if (PuzzleController.IsCouponEvent) SetCoupons(); //HB 
            _boardView.Show(BoardViewShowComplete);
        }

        private void CreateLockCellQuests()
        {
            // Set LockModuleQuest counts
            foreach (var lockQuestPair in _boardData.lockQuestPairs)
            {
                MyCell cell = _board.Cells[lockQuestPair.position.x, lockQuestPair.position.y];
                var lockQuestModule = cell.GetModule<LockQuestModule>();
                if (lockQuestModule != null)
                {
                    lockQuestModule.SetColor(lockQuestPair.color);
                    lockQuestModule.SetInitialCount(lockQuestPair.count);
                    lockQuestModule.AddCell(cell);
                    AddNeighborCells(lockQuestModule, cell);
                    UnityEngine.Debug.Log($"Lock at position {cell.position} has {lockQuestModule.GetCellCount()} cells");
                }
                else
                {
                    UnityEngine.Debug.LogError($"Cell at position {lockQuestPair.position} does not have LockModuleQuest");
                }
            }
        }

        private void AddNeighborCells(LockQuestModule lockModuleQuest, MyCell cell)
        {
            // UnityEngine.Debug.Log($"AddNeighborCells: Lock cell has {cell.Adjacents.Count} adjacent");
            foreach (var adjacent in cell.Adjacents)
            {
                LockModule neighborLock = adjacent.GetModule<LockModule>();
                // if (neighborLock != null) UnityEngine.Debug.Log($"AddNeighborCells: {lockModuleQuest.color} vs. {neighborLock.color}");
                if (neighborLock != null && neighborLock.color == lockModuleQuest.color)
                {
                    if (lockModuleQuest.AddCell(adjacent))
                    {
                        AddNeighborCells(lockModuleQuest, adjacent);
                    }
                }
            }
        }

        private void CreateHandPlacedBoosters()
        {
            foreach (var cell in _board.Cells)
            {
                if (cell.IsVisible)
                {
                    var boosterModule = cell.GetModule<BoosterModule>();
                    if (boosterModule != null)
                    {
                        if (boosterModule.BoosterType != BoosterType.JumboBlock)
                        {
                            boosterModule.SetBoard(_board);
                            boosterModule.SetData(boosterModule.BoosterType, cell.position);
                        }
                        else
                        {
                            cell.RemoveModule<BoosterModule>();
                            BlockModule block = cell.GetModule<BlockModule>();
                            if (block != null)
                            {
                                block.SetJumbo();
                            }
                        }
                    }
                }
            }
        }

        private void SetIronWallNeighbours()
        {
            int width = _board.Width;
            int height = _board.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    MyCell cell = _board.Cells[x, y];
                    IronWallModule ironWallModule = cell.GetModule<IronWallModule>();
                    if (ironWallModule != null)
                    {
                        switch (ironWallModule.Restriction.locks[0].direction)
                        {
                            case Direction.Left:
                                if (_board.IsInBounds(x - 1, y)) ironWallModule.SetNeighbour(_board.Cells[x - 1, y]);
                                break;
                            case Direction.Top:
                                if (_board.IsInBounds(x, y + 1)) ironWallModule.SetNeighbour(_board.Cells[x, y + 1]);
                                break;
                            case Direction.Bottom:
                                if (_board.IsInBounds(x, y - 1)) ironWallModule.SetNeighbour(_board.Cells[x, y - 1]);
                                break;
                            case Direction.Right:
                                if (_board.IsInBounds(x + 1, y)) ironWallModule.SetNeighbour(_board.Cells[x + 1, y]);
                                break;
                            case Direction.Center:
                            case Direction.All:
                            case Direction.None:
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void BoardViewShowComplete()
        {
            CreateHandPlacedBoosters();
            _board.AddInitialBoosters();
            _boardView.ShowNextTutorial();
            onCreateBoardFinished?.Invoke();
            onCreateBoardFinished = null;
            Log.Debug("Match3", $"Finishing Board creation");
        }

        private void SetCoupons()
        {
            var couponCount = _random.Next(Setting.MinInitCoupons, Setting.MaxInitCoupons + 1);
            // Check if there are enough simple blocks on board
            int simpleBlocks = 0;
            foreach (var cell in _board.Cells)
            {
                if (cell.IsVisible &&
                    cell.IsLocked(ActionType.Swap, Direction.Center) == false)
                {
                    BlockModule block = cell.GetModule<BlockModule>();
                    if (block != null && block.id.Contains(BlockIDs.Simple))
                    {
                        simpleBlocks++;
                        if (simpleBlocks >= couponCount) break;
                    }
                }
            }
            if (simpleBlocks < couponCount) return;

            for (int i = 0; i < couponCount; i++)
            {
                int tryCount = 0;
                // get a random cell that has a swapable block
                MyCell cell = GetRandomCell();
                BlockModule block = cell.GetModule<BlockModule>();
                while (IsNotValidBlockForCoupon(cell, block))
                {
                    tryCount++;
                    cell = GetRandomCell();
                    block = cell.GetModule<BlockModule>();
                    if (tryCount >= 1000) break;
                }

                if (IsNotValidBlockForCoupon(cell, block) == false)
                    block.SetCoupon(true);
            }
        }

        private static bool IsNotValidBlockForCoupon(MyCell cell, BlockModule block)
        {
            return block == null
                || block.id.Contains(BlockIDs.Simple) == false
                || cell.IsLocked(ActionType.Swap, Direction.Center)
                || block.HasCoupon == true;
        }

        private MyCell GetRandomCell()
        {
            return _board.Cells[_random.Next(0, _board.Width), _random.Next(0, _board.Height)];
        }

        private void CreateInitialBlocks()
        {
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    MyCell cell = _board.Cells[x, y];
                    _boardView.CreateCellView(cell);

                    if (CanCreateBlockInCell(cell))
                    {
                        if (IsRestrictedToCreateBlock(cell) == false || cell.Contains<LockModule>() || cell.Contains<LockQuestModule>())
                        {
                            BlockModule initialBlockModule = cell.GetModule<BlockModule>();
                            if (initialBlockModule != null)
                            {
                                cell.AddBlock(initialBlockModule);
                            }
                        }
                    }
                }
            }
        }

        private void CreateBlocksForEmptyCells()
        {
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    MyCell cell = _board.Cells[x, y];
                    if (CanCreateBlockInCell(cell))
                    {
                        if (IsRestrictedToCreateBlock(cell) == false)
                        {
                            if (cell.flow != null && cell.flow.Incommings.Count != 0)
                            {
                                BlockModule initialBlockModule = cell.GetModule<BlockModule>();
                                if (initialBlockModule == null)
                                {
                                    CreateInitialBlockModule(cell);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetAdjacent()
        {
            for (int x = 0; x < _board.Width; x++)
                for (int y = 0; y < _board.Height; y++)
                {
                    MyCell cell = _board.Cells[x, y];
                    SetAdjacents(cell);
                }
        }

        private void SetAdjacents(MyCell cell)
        {
            if (_board.HasEmptyCell(cell.position) == false)
            {
                return;
            }

            List<MyCell> adjacents = new List<MyCell>(4);
            Point[] directions =
            {
                new Point(cell.position.x - 1, cell.position.y),
                new Point(cell.position.x + 1, cell.position.y),
                new Point(cell.position.x, cell.position.y + 1),
                new Point(cell.position.x, cell.position.y - 1)
            };

            for (int i = 0; i < directions.Length; i++)
            {
                if (_board.HasEmptyCell(directions[i]))
                {
                    Point p = directions[i];
                    adjacents.Add(_board.Cells[p.x, p.y]);
                }
            }

            cell.SetAdjacents(adjacents);
        }

        private static bool IsRestrictedToCreateBlock(MyCell cell)
        {
            return cell.IsLocked(ActionType.Spawn, Direction.Center);
        }

        private void CreateInitialBlockModule(MyCell cell)
        {
            BlockType bt = BlockType.None;
            if (allBlockTypes.Length == 0) allBlockTypes = _blockFactory.GetLevelBlockTypes();
            BlockType[] randomizedBlockTypes = allBlockTypes.OrderBy(x => _random.Next()).ToArray();
            foreach (var blockType in randomizedBlockTypes)
            {
                if (HasSimilar(cell.position, blockType) == false && blockType != BlockType.None)
                {
                    bt = blockType;
                    break;
                }
            }
            if (bt == BlockType.None) bt = randomizedBlockTypes[0];
            _blockFactory.CreateBlock(cell, bt);
        }

        private bool HasSimilar(Point pos, BlockType bt)
        {
            return pos.x >= 2 && BlockHasTwoSimilarInRowLeft(pos.x, pos.y, bt) ||
                   pos.x < _board.Width - 2 && BlockHasTwoSimilarInRowRight(pos.x, pos.y, bt) ||
                   pos.x > 0 && pos.x < _board.Width - 1 && BlockHasOneNeighbourHorizontal(pos.x, pos.y, bt) ||
                   pos.y >= 2 && BlockHasTwoSimilarInColumnDown(pos.x, pos.y, bt) ||
                   pos.y < _board.Height - 2 && BlockHasTwoSimilarInColumnTop(pos.x, pos.y, bt) ||
                   pos.y > 0 && pos.y < _board.Height - 1 && BlockHasOneNeighbourVertical(pos.x, pos.y, bt) ||
                   pos.x >= 1 && pos.y >= 1 && pos.x < _board.Width && pos.y < _board.Height && BlockIsInSquare(pos.x, pos.y, bt);
        }

        private static bool CanCreateBlockInCell(MyCell cell)
        {
            return cell.IsVisible && cell.Contains<CannonModule>() == false;
        }

        #endregion

        #region Private Methods
        private bool BlockHasTwoSimilarInColumnDown(int x, int y, BlockType bt)
        {
            BlockModule block = _board.Cells[x, y - 2].GetModule<BlockModule>();
            if (block == null) return false;
            BlockModule otherBlock = _board.Cells[x, y - 1].GetModule<BlockModule>();
            if (otherBlock == null) return false;

            return block.blockType.Equals(bt) && otherBlock.blockType.Equals(bt);
        }

        private bool BlockHasTwoSimilarInColumnTop(int x, int y, BlockType bt)
        {
            BlockModule block = _board.Cells[x, y + 2].GetModule<BlockModule>();
            if (block == null) return false;
            BlockModule otherBlock = _board.Cells[x, y + 1].GetModule<BlockModule>();
            if (otherBlock == null) return false;
            return block.blockType.Equals(bt) && otherBlock.blockType.Equals(bt);
        }

        private bool BlockHasOneNeighbourVertical(int x, int y, BlockType bt)
        {
            BlockModule block = _board.Cells[x, y + 1].GetModule<BlockModule>();
            if (block == null) return false;
            BlockModule otherBlock = _board.Cells[x, y - 1].GetModule<BlockModule>();
            if (otherBlock == null) return false;
            return block.blockType.Equals(bt) && otherBlock.blockType.Equals(bt);
        }

        private bool BlockHasTwoSimilarInRowLeft(int x, int y, BlockType bt)
        {
            BlockModule block = _board.Cells[x - 2, y].GetModule<BlockModule>();
            if (block == null) return false;

            BlockModule otherBlock = _board.Cells[x - 1, y].GetModule<BlockModule>();
            if (otherBlock == null) return false;

            return block.blockType.Equals(bt) && otherBlock.blockType.Equals(bt);
        }
        private bool BlockHasTwoSimilarInRowRight(int x, int y, BlockType bt)
        {
            BlockModule block = _board.Cells[x + 2, y].GetModule<BlockModule>();
            if (block == null) return false;

            BlockModule otherBlock = _board.Cells[x + 1, y].GetModule<BlockModule>();
            if (otherBlock == null) return false;

            return block.blockType.Equals(bt) && otherBlock.blockType.Equals(bt);
        }

        private bool BlockHasOneNeighbourHorizontal(int x, int y, BlockType bt)
        {
            BlockModule block = _board.Cells[x + 1, y].GetModule<BlockModule>();
            if (block == null) return false;
            BlockModule otherBlock = _board.Cells[x - 1, y].GetModule<BlockModule>();
            if (otherBlock == null) return false;
            return block.blockType.Equals(bt) && otherBlock.blockType.Equals(bt);
        }

        private bool BlockIsInSquare(int x, int y, BlockType bt)
        {
            BlockModule left = _board.Cells[x - 1, y].GetModule<BlockModule>();
            BlockModule right = null;
            if (_board.IsInBounds(x + 1, y)) right = _board.Cells[x + 1, y].GetModule<BlockModule>();
            BlockModule down = _board.Cells[x, y - 1].GetModule<BlockModule>();
            BlockModule downLeft = _board.Cells[x - 1, y - 1].GetModule<BlockModule>();
            BlockModule downRight = null;
            if (_board.IsInBounds(x + 1, y - 1)) downRight = _board.Cells[x + 1, y - 1].GetModule<BlockModule>();
            BlockModule top = null;
            if (_board.IsInBounds(x, y + 1)) top = _board.Cells[x, y + 1].GetModule<BlockModule>();
            BlockModule topLeft = null;
            if (_board.IsInBounds(x - 1, y + 1)) topLeft = _board.Cells[x - 1, y + 1].GetModule<BlockModule>();
            BlockModule topRight = null;
            if (_board.IsInBounds(x + 1, y + 1)) topRight = _board.Cells[x + 1, y + 1].GetModule<BlockModule>();
            bool similarLeft = left != null ? left.blockType.Equals(bt) : false;
            bool similarRight = right != null ? right.blockType.Equals(bt) : false;
            bool similarDown = down != null ? down.blockType.Equals(bt) : false;
            bool similarDownLeft = downLeft != null ? downLeft.blockType.Equals(bt) : false;
            bool similarDownRight = downRight != null ? downRight.blockType.Equals(bt) : false;
            bool similarTop = top != null ? top.blockType.Equals(bt) : false;
            bool similarTopLeft = topLeft != null ? topLeft.blockType.Equals(bt) : false;
            bool similarTopRight = topRight != null ? topRight.blockType.Equals(bt) : false;
            if (similarLeft && similarDown && similarDownLeft) return true;
            if (similarRight && similarDown && similarDownRight) return true;
            if (similarTop && similarTopLeft && similarLeft) return true;
            if (similarTop && similarTopRight && similarRight) return true;
            return false;
        }

        #endregion
    }
    
}