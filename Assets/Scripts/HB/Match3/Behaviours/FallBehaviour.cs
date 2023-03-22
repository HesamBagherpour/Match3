using HB.Match3.Block;
using HB.Match3.Modules;

namespace HB.Match3.Behaviours
{
 public class FallBehaviour
    {
        #region Private Fields

        private readonly Match3MainBoard.Board _board;
        private readonly BlockFactory _blockFactory;
        private int _fallCounter;

        #endregion

        #region  Constructors

        public FallBehaviour(Match3MainBoard.Board board, BlockFactory blockFactory)
        {
            _board = board;
            _blockFactory = blockFactory;
        }

        public bool IsFinished;
        #endregion

        public void Fall()
        {
            IsFinished = false;
            FallAllMainFlows();
        }

        private void FallAllMainFlows()
        {
            _fallCounter = 0;
            var flows = _board.BoardFlow.GetFlows();
            for (int i = 0; i < flows.Count; i++)
            {
                var flow = flows[i];
                FallOneStep(flow, true);
            }
            if (_fallCounter == 0)
            {
                IsFinished = true;
            }
        }

        private void FallOneStep(Flow flow, bool includeProvider)
        {
            bool somethingfalled = false;
            for (int i = flow.Cells.Count - 1; i >= 0; i--)
            {
                MyCell cell = flow.Cells[i];
                if (IsEmptyCell(cell))
                {
                    MyCell provider = cell.flow.GetCellProvider(cell, includeProvider);
                    if (provider != null)
                    {
                        somethingfalled = true;
                        ExchangeBlock(provider, cell);
                    }
                }
            }
            for (int i = 0; i < flow.subFlows.Count; i++)
            {
                if (somethingfalled == false)
                {
                    FallOneStep(flow.subFlows[i], true);
                }
                else
                {
                    FallOneStep(flow.subFlows[i], false);
                }
            }
        }
        private void ExchangeBlock(MyCell provider, MyCell emptyCell)
        {
            BlockModule block = provider.GetModule<BlockModule>();
            if (block == null && provider.Contains<SpawnerModule>())
            {
                _blockFactory.CreateFallingBlock(provider);
                block = provider.GetModule<BlockModule>();
            }
            if (block != null)
            {
                _fallCounter++;
                block.Fall(emptyCell.position, FallStepComplete);
                provider.ExchangeBlocks(emptyCell);
            }
        }

        private static bool IsEmptyCell(MyCell cell)
        {
            return cell.IsVisible &&
                   cell.Contains<BlockModule>() == false;
        }

        private void FallStepComplete()
        {
            _fallCounter--;
            if (_fallCounter == 0)
            {
                FallAllMainFlows();
            }
        }
    }
}