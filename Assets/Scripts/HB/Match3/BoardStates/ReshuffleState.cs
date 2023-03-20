using System;
using System.Collections.Generic;
using System.Text;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Logger;
using HB.Match3;
using HB.Match3.Cells.Modules;
using HB.Match3.View;

namespace Garage.Match3.BoardStates
{
    public class ReshuffleState : BoardState
    {
        private readonly MatchPredictor _matchPredictor;
        private readonly Random _random;
        private int _blockMovedCount;
        private bool _eventDispatched = false;
        private readonly List<Cell> reShuffleCells;
        private bool _reshuffled;
        private bool _finished;

        public ReshuffleState(MatchPredictor matchPredictor, Random random)
        {
            _matchPredictor = matchPredictor;
            reShuffleCells = new List<Cell>();
            _random = random;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            _eventDispatched = false;
            _finished = false;
            reShuffleCells.Clear();
            CheckPossibleMatch();
            _reshuffled = false;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_finished)
            {
                Finished();
                _finished = false;
            }
        }
        protected override void OnExit()
        {
            base.OnExit();
        }

        private void CheckPossibleMatch()
        {
            if (HasPossibleMatch())
            {
                if (_reshuffled)
                {
                    MoveBlockPositions();
                }
                else
                {
                    _finished = true;
                }
            }
            else
            {
                if (HasEnoughColors() == false)
                {
                    ChangeColors();
                }
                ReShuffle();
            }
        }

        private void MoveBlockPositions()
        {
            _blockMovedCount = 0;
            int width = Agent.Width;
            int height = Agent.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = Cells[x, y];
                    BlockModule blockModule = cell.GetModule<BlockModule>();
                    if (cell.IsVisible &&
                        cell.IsLocked(ActionType.Move, Direction.Center) == false &&
                        blockModule != null &&
                        blockModule.blockType.color != BlockColor.None)
                    {
                        _blockMovedCount++;
                        blockModule.Reshuffle(cell.position, OnMoveComplete);
                    }
                }
            }
        }
        private void OnMoveComplete()
        {
            _blockMovedCount--;
            if (_blockMovedCount == 0)
            {
                _finished = true;
            }
        }

        private bool HasPossibleMatch()
        {
            var hasPossibleMatch = false;
            // Log.Debug("Match3", $"Checking reshuffle {_reshuffleTryCount} times");
            int width = Agent.Width;
            int height = Agent.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = Cells[x, y];
                    BlockModule blockModule = cell.GetModule<BlockModule>();
                    if (cell.IsVisible &&
                        cell.IsLocked(ActionType.Move, Direction.Center) == false &&
                        blockModule != null &&
                        blockModule.blockType.id.Contains(BlockIDs.flower) == false &&
                        blockModule.blockType.color != BlockColor.None)
                    {
                        if (reShuffleCells.Contains(cell) == false ) reShuffleCells.Add(cell);
                        for (int i = 0; i < cell.Adjacents.Count; i++)
                        {
                            if (cell.position.x == 0 && cell.position.y == 0)
                            {
                                var t = true;
                            }
                            Cell otherCell = cell.Adjacents[i];
                            if (_matchPredictor.WillMatchBySwap(cell, otherCell))
                            {
                                hasPossibleMatch = true;
                                break;
                            }
                        }
                    }

                    if (hasPossibleMatch) break;
                }

                if (hasPossibleMatch) break;
            }
            return hasPossibleMatch;
        }

        private void ReShuffle()
        {
            _reshuffled = true;
            if (!_eventDispatched)
            {
                _eventDispatched = true;
                Agent.InvokeReshuffle();
            }
            var tmpCellList = new List<Cell>();
            tmpCellList.AddRange(reShuffleCells);

            int count = tmpCellList.Count;
            while (count != 0)
            {
                Cell currentCell = tmpCellList[0];
                tmpCellList.RemoveAt(0);
                count--;

                if (count <= 0) break;
                int randomTarget = _random.Next(0, count);
                Cell targetCell = tmpCellList[randomTarget];
                tmpCellList.RemoveAt(randomTarget);
                count--;
                currentCell.ExchangeBlocks(targetCell);
                // Log.Debug("Match3", $"Reshuffle exchanging blocks at {currentCell} ,{targetCell}");
            }
            CheckPossibleMatch();
        }

        private bool HasEnoughColors()
        {
            bool alreadyHave = false;
            List<BlockType> checkedBlockTypes = new List<BlockType>();
            for (int i = 0; i < reShuffleCells.Count; i++)
            {
                var bt = reShuffleCells[i].GetBlockType();
                if (!checkedBlockTypes.Contains(bt))
                {
                    int counter = 1;
                    for (int j = i + 1; j < reShuffleCells.Count; j++)
                    {
                        if (reShuffleCells[j].GetBlockType().Equals(bt))
                        {
                            counter++;
                        }
                    }

                    if (counter >= 3)
                    {
                        // we have more than 3 blocks for a blocktype
                        alreadyHave = true;
                        break;
                    }

                    checkedBlockTypes.Add(bt);
                }
            }
            return alreadyHave;
        }

        private void ChangeColors()
        {
            if (reShuffleCells.Count != 0)
            {
                // TODO: Potential bug.
                Log.Debug("Match3", $"Change colors of first 3 blockTypes");
                Cell cell = reShuffleCells[0];
                BlockType firstBt = cell.GetBlockType();

                Cell secondCell = reShuffleCells[1];
                BlockModule secondBm = secondCell.GetModule<BlockModule>();
                BlockModuleView secondBmView = (BlockModuleView)secondBm.view;
                secondBmView.SetBlockType(firstBt);
                secondBm.Setup(firstBt);

                Cell thirdCell = reShuffleCells[2];
                BlockModule thirdBm = thirdCell.GetModule<BlockModule>();
                BlockModuleView thirdBmView = (BlockModuleView)thirdBm.view;
                thirdBmView.SetBlockType(firstBt);
                thirdBm.Setup(firstBt);
            }
        }
    }
}