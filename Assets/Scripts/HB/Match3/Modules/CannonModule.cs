using System;
using System.Collections.Generic;
using HB.Match3.Board;
using HB.Match3.Cell;
using HB.Packages.Logger;
using HB.Match3.View;

namespace HB.Match3.Modules
{
    public class CannonModule : BaseModule
    {
        public Direction direction;
        private readonly List<MyCell> _cells;
        private CannonModuleView _cannonModuleView;
        private Action _onFireComplete;
        private int cellIndex;
        public const string LayerName = "Spotlight-Cannon";

        public CannonModule()
        {
            layerName = LayerName;
            _cells = new List<MyCell>();
        }

        public void SetCells(List<MyCell> cells)
        {
            _cells.Clear();
            _cells.AddRange(cells);
        }

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            _cannonModuleView = (CannonModuleView)view;
        }

        public override void Clear(MyCell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            // we send back null on finish to prevent removing module
            onFinished?.Invoke(null);
        }

        protected override void OnSetup()
        {
        }

        public void Fire(Action onFireComplete)
        {
            if (_cells.Count == 0)
            {
                onFireComplete?.Invoke();
                return;
            }

            cellIndex = 0;
            BoardView.PlayAudio("CannonFire", 0.3f);
            _onFireComplete = onFireComplete;
            _cannonModuleView.Fire(_cells, OnOneCellPassed);
        }

        private void OnFireComplete()
        {
            _cannonModuleView.FireComplete();
            _onFireComplete?.Invoke();
            _onFireComplete = null;
        }

        private void OnOneCellPassed()
        {
            // Log.Debug("Match3", $"Cannon {cellIndex} passed");
            var cell = _cells[cellIndex];
            if (cell.IsLocked(ActionType.HitBlock, Direction.Center) == false &&
                cell.IsRestrictedBlock(ActionType.HitBlock) == false)
            {
                BlockModule blockModule = cell.GetModule<BlockModule>();
                if (blockModule != null)
                {
                    //HB
                    // if (QuestGiver.IsInQuest(blockModule))
                    // {
                    //     blockModule.AddCount(1);
                    // }
                    //HB
                }
            }

            cellIndex++;

            if (cellIndex >= _cells.Count)
            {
                _cannonModuleView.ShowFireEnd(OnFireComplete);
            }
        }
    }
}
