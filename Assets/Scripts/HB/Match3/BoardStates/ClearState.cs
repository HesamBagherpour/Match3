using System;
using System.Diagnostics;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.Behaviours;
using HB.Match3.Cells.Modules;
using HB.Match3.View;


namespace Garage.Match3.BoardStates
{
    public class ClearState : BoardState
    {
        #region Private Fields
        private int _cellClearCount;
        private IBoardView _boardView;
        private BlockFactory _blockFactory;
        // private List<MatchInfo> tmpMatchInfos;

        public ClearState(IBoardView boardView, BlockFactory blockFactory)
        {
            _boardView = boardView;
            _blockFactory = blockFactory;
        }

        #endregion

        #region Unity

        #endregion

        #region Protected Methods

        protected override void OnEnter()
        {
            base.OnEnter();
            ClearBoard();
        }

        protected override void OnExit()
        {
            base.OnExit();
            _boardView.PlayCollectCounter();
        }

        private void ClearBoard()
        {
            _cellClearCount = 0;
            // if (tmpMatchInfos != null || tmpMatchInfos.Count == 0) tmpMatchInfos.Clear();
            // tmpMatchInfos = TempMatchInfos();
            // CheckForNestedBoxes();
            ClearAllCells();
        }

        private void ClearAllCells()
        {
            // We call Clear for all cells because adjacest hits are not in matchinfo lists
            int width = Agent.Width;
            int height = Agent.Height;
            UpdateParquets();
            ClearNalbakies();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = Cells[x, y];
                    if (cell.IsVisible && cell.HitType != HitType.None)
                    {
                        _cellClearCount++;
                        cell.Clear(OnClearFinished);
                    }
                }
            }
        }


        private void UpdateParquets()
        {
            bool hasParquetInTotal = false;
            foreach (var matchInfo in Agent.MatchInfos)
            {
                if (matchInfo.matchType != MatchType.Booster && matchInfo.matchType != MatchType.ExittingPlant && matchInfo.matchType != MatchType.Nalbaki)
                {
                    bool hasParquet = false;
                    foreach (var cell in matchInfo.MatchedCells)
                    {
                        if (cell.Contains<ParquetModule>())
                        {
                            hasParquet = true;
                            break;
                        }
                    }
                    if (hasParquet)
                    {
                        foreach (var cell in matchInfo.MatchedCells)
                        {
                            if (cell.Contains<ParquetModule>() == false) hasParquetInTotal = true;
                            cell.MarkForParquet();
                        }
                    }
                }
            }
            if (hasParquetInTotal) BoardView.PlayAudio("parquet");
        }

        private void ClearNalbakies()
        {
            foreach (var cell in Cells)
            {
                if (cell.IsVisible && cell.IsLocked(ActionType.HitBlock, Direction.Center) == false)
                {
                    BlockModule nalbaki = cell.GetModule<BlockModule>();
                    if (nalbaki != null && nalbaki.blockType.id.Equals(BlockIDs.Nalbaki))
                    {
                        for (int i = 0; i < cell.Adjacents.Count; i++)
                        {
                            Cell adjacent = cell.Adjacents[i];
                            if (Agent.MatchInfos.FindIndex(x => x.MatchedCells.FindIndex(y => y == adjacent) != -1
                                                                && (x.matchType == MatchType.Horizontal
                                                                || x.matchType == MatchType.Vertical
                                                                || x.matchType == MatchType.Square)) != -1)
                            {
                                // adjacent is matched with swap
                                var dmg = 1;
                                _cellClearCount++;
                                cell.ClearBlock(ref dmg, HitType.Direct, module => OnClearFinished(cell));
                            }
                        }
                    }
                }
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_cellClearCount == 0)
            {
                Finished();
            }
        }

        private void OnClearFinished(Cell cell)
        {
            --_cellClearCount;
        }
        private void OnClearFlowerFinished(Cell cell)
        {
            --_cellClearCount;
        }

        #endregion
    }
}