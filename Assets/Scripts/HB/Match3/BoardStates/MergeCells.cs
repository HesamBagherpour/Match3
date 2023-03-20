using System;
using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using HB.Match3;
using HB.Match3.Cells.Modules;

namespace Garage.Match3.BoardStates
{
    public class MergeCells : BoardState
    {
        private bool _finished;
        private int _mergeCounter;

        protected override void OnEnter()
        {
            base.OnEnter();
            _mergeCounter = 0;
            MergeBoosterCellViews();
            MergeMatchedBoxesNotInBooster();
            if (_mergeCounter == 0)
            {
                _finished = true;
            }
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

        private void MergeBoosterCellViews()
        {
            List<BoosterInfo> boosterInfos = Agent.BoosterInfo;
            for (int i = 0; i < boosterInfos.Count; i++)
            {
                BoosterInfo boosterInfo = boosterInfos[i];
                Cell freeCellToMerge = GetFreeCellToMerge(boosterInfo.OriginCell, boosterInfo.cells);
                boosterInfo.OriginCell = freeCellToMerge;
                for (int j = 0; j < boosterInfo.cells.Count; j++)
                {
                    Cell cell = boosterInfo.cells[j];
                    if (cell != freeCellToMerge && cell.flow != null && cell.IsLocked(ActionType.HitBlock, Direction.Center) == false)
                    {
                        BlockModule block = cell.GetModule<BlockModule>();
                        if (block != null && block.IsJumbo == false)
                        {
                            _mergeCounter++;
                            block.MoveTo(freeCellToMerge.position, OnMergeComplete);
                            block.MergeIntoOthers();
                        }
                    }
                }
            }
        }
        private void MergeMatchedBoxesNotInBooster()
        {
            // after checking all boosters, we check for matched infos that are not in booster infos
            var tmpMatchInfos = Agent.MatchInfos;
            foreach (var matchInfo in tmpMatchInfos)
            {
                if (matchInfo.MatchedCells.Count == 3)
                {
                    Cell freeCellToMerge = GetFreeCellToMerge(matchInfo.OriginCell, matchInfo.MatchedCells);
                    matchInfo.OriginCell = freeCellToMerge;
                    var blockModule = freeCellToMerge.GetModule<BlockModule>();
                    if (blockModule != null && blockModule.id.Contains(BlockIDs.Box))
                    {
                        if (CellIsInBoosterInfos(freeCellToMerge) == false)
                        {
                            foreach (var otherCell in matchInfo.MatchedCells)
                            {
                                if (otherCell != freeCellToMerge && otherCell.IsLocked(ActionType.HitBlock, Direction.Center) == false)
                                {
                                    _mergeCounter++;
                                    BlockModule otherBlock = otherCell.GetModule<BlockModule>();
                                    otherBlock.MoveTo(freeCellToMerge.position, OnMergeComplete);
                                    otherBlock.MergeIntoOthers();
                                }
                            }
                        }
                    }
                }
            }
        }
        
        
        
        
        

        private bool CellIsInBoosterInfos(Cell originCell)
        {
            foreach (var boosterInfo in Agent.BoosterInfo)
            {
                foreach (var cell in boosterInfo.cells)
                {
                    if (cell == originCell) return true;
                }
            }
            return false;
        }

        private Cell GetFreeCellToMerge(Cell originCell, List<Cell> cells)
        {
            BlockModule freeBlock = originCell.GetModule<BlockModule>();
            if (freeBlock != null && originCell.IsLocked(ActionType.HitBlock, Direction.Center) == false)
            {
                return originCell;
            }
            else
            {
                foreach (var cell in cells)
                {
                    freeBlock = cell.GetModule<BlockModule>();
                    if (freeBlock != null && cell.IsLocked(ActionType.HitBlock, Direction.Center) == false) return cell;
                }
            }
            return originCell;
        }

        private void OnMergeComplete()
        {
            _mergeCounter--;
            if (_mergeCounter == 0)
            {
                _finished = true;
            }
        }
    }
}