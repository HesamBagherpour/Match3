using Garage.Match3.Cells.Modules;
using HB.Match3.Behaviours;
using HB.Match3.Cells.Modules;

namespace Garage.Match3.BoardStates
{
    public class AddJumboState : BoardState
    {
        private bool _finished;
        private readonly BlockFactory _blockFactory;
        private readonly DetectJumboState _detectJumboState;

        public AddJumboState(BlockFactory blockFactory, DetectJumboState detectJumboState)
        {
            _blockFactory = blockFactory;
            _detectJumboState = detectJumboState;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            foreach (var item in _detectJumboState._cachedJumbos)
            {
                var cell = Agent.Cells[item.pos.x, item.pos.y];
                if (cell.Contains<BlockModule>() == false) _blockFactory.CreateBlock(cell, item.blockType);
                var blockModule = cell.GetModule<BlockModule>();
                blockModule.SetJumbo();
                blockModule.AddCount(item.count - 1);
            }
            _finished = true;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_finished) Finished();
        }

    }
}