using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3.Behaviours;

namespace Garage.Match3.BoardStates
{
    public class AddBoosterState : BoardState
    {
        private int _clearCount;
        private IBoardView _boardView;
        private BlockFactory _blockFactory;

        public AddBoosterState(IBoardView boardView, BlockFactory blockFactory)
        {
            _boardView = boardView;
            _blockFactory = blockFactory;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            AddBoosters();
            _clearCount = 0;
        }



        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_clearCount == 0)
            {
                Finished();
            }
        }

        // private void OnBoosterHit(BoosterModule booster)
        // {
        //     if (booster != null) _timeout = 0.7f;
        //     _boardView.PlayCollectCounter();
        //     _count--;
        // }

        // private void ClearBoard()
        // {
        //     _count = -1;
        //     _clearCount = 0;
        //     _boardView.PlayCollectCounter();
        //     for (int x = 0; x < Agent.Width; x++)
        //     {
        //         for (int y = 0; y < Agent.Height; y++)
        //         {
        //             Cell cell = Cells[x, y];
        //             if (cell.IsVisible)
        //             {
        //                 _clearCount++;
        //                 BlockType nestedBlockType = Agent.HasNestedBlockType(cell);
        //                 if (nestedBlockType != BlockType.None) cell.nestedBlockType = nestedBlockType;
        //                 cell.Clear(OnClearFinished);
        //             }
        //         }
        //     }
        // }

        // private void OnClearFinished(Cell cell)
        // {
        //     if (cell.nestedBlockType != BlockType.None && cell.Contains<BlockModule>() == false)
        //     {
        //         _blockFactory.CreateBlock(cell, cell.nestedBlockType);
        //     }
        //     --_clearCount;
        // }

        private void AddBoosters()
        {
            for (int i = 0; i < Agent.BoosterInfo.Count; i++)
            {
                BoosterInfo boosterInfo = Agent.BoosterInfo[i];
                Cell originCell = boosterInfo.OriginCell;
                Agent.GetOrAddBoosterModule(originCell, boosterInfo.Type);
            }
        }
    }
}