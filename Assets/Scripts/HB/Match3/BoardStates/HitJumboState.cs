using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.Cells.Modules.Boosters;
using HB.Match3;
using HB.Match3.Cells.Modules;

namespace Garage.Match3.BoardStates
{
    public class HitJumboState : BoardState
    {
        private bool _finished;
        public bool HasJumbo { get; private set; }

        private int activeJumboCounter;

        protected override void OnEnter()
        {
            base.OnEnter();
            HasJumbo = false;
            activeJumboCounter = 0;
            int width = Agent.Width;
            int height = Agent.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = Cells[x, y];
                    if (cell.IsVisible)
                    {
                        if (cell.HitType == HitType.Direct)
                        {
                            var blockModule = cell.GetModule<BlockModule>();
                            if (blockModule != null && blockModule.IsJumbo)
                            {
                                activeJumboCounter++;
                                var jumboBehaviour = new JumboBooster(Agent, cell.position, 1);
                                jumboBehaviour.Activate();
                                if (HasJumbo == false) HasJumbo = true;
                            }
                        }
                    }
                }
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