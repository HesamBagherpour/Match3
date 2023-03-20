using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules.Boosters;
using Garage.Match3.View;
using HB.Match3.Behaviours;

namespace Garage.Match3.BoardStates
{
    public class ActiveJumboState : BoardState
    {
        private int clearCounter;
        private readonly List<Point> _allHitPositions;
        private readonly BlockFactory _blockFactory;
        private readonly IBoardView _boardView;

        public ActiveJumboState(BlockFactory blockFactory, IBoardView boardView)
        {
            _allHitPositions = new List<Point>();
            _blockFactory = blockFactory;
            _boardView = boardView;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            // Log.Debug("ActiveJumboState", $"Enter state");
            clearCounter = -1;
            _allHitPositions.Clear();
            // HitAllJumbos();
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (clearCounter == 0)
            {
                Finished();
            }
        }

        protected override void OnExit()
        {
            // Log.Debug("ActiveJumboState", $"Exitting state");
            base.OnExit();
        }

        // private void HitAllJumbos()
        // {
        //     bool somethingHit = false;
        //     int width = Agent.Width;
        //     int height = Agent.Height;
        //     for (int i = 0; i < width; i++)
        //     {
        //         for (int j = 0; j < height; j++)
        //         {
        //             var cell = Cells[i, j];
        //             if (cell.IsVisible && cell.JumboCleared)
        //             {
        //                 cell.JumboCleared = false;
        //                 somethingHit = true;
        //                 var jumboBehaviour = new JumboBooster(Agent, cell.position, 1);
        //                 var hitPositions = jumboBehaviour.GetCellPositions();
        //                 foreach (var pos in hitPositions)
        //                 {
        //                     if (_allHitPositions.Contains(pos) == false) _allHitPositions.Add(pos);
        //                 }
        //                 jumboBehaviour.Activate();
        //                 // Log.Debug("ActiveJumboState", $"Jumbo executed!");
        //             }
        //         }
        //     }
        //     if (somethingHit) ClearBoard();
        //     else
        //     {
        //         // Log.Debug("ActiveJumboState", $"Exitting for no jumbo found");
        //         clearCounter = 0;
        //     }
        // }

        // private void ClearBoard()
        // {
        //     // Log.Debug("ActiveJumboState", $"Clearing Board started");
        //     SetNestedBlockTypes();
        //     _boardView.PlayCollectCounter();

        //     // Clear cells
        //     clearCounter = 0;
        //     for (int i = 0; i < _allHitPositions.Count; i++)
        //     {
        //         Point pos = _allHitPositions[i];
        //         var cell = Cells[pos.x, pos.y];
        //         if (cell.IsVisible)
        //         {
        //             clearCounter++;
        //             cell.Clear(OnClearFinished);
        //         }
        //     }
        // }

        // private void SetNestedBlockTypes()
        // {
        //     // Set nested blocks before clear cells
        //     foreach (var pos in _allHitPositions)
        //     {
        //         var cell = Agent.Cells[pos.x, pos.y];
        //         BlockType nestedBlockType = Agent.HasNestedBlockType(cell);
        //         if (nestedBlockType != BlockType.None)
        //         {
        //             cell.nestedBlockType = nestedBlockType;
        //         }
        //     }
        // }

        // private void OnClearFinished(Cell cell)
        // {
        //     if (cell.nestedBlockType != BlockType.None)
        //     {
        //         _blockFactory.CreateBlock(cell, cell.nestedBlockType);
        //     }
        //     clearCounter--;
        // }
    }
}