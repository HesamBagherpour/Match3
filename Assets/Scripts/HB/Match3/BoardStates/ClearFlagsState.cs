using Garage.Match3.Cells;

namespace Garage.Match3.BoardStates
{
    public class ClearFlagsState : BoardState
    {
        private bool _finished;
        protected override void OnEnter()
        {
            base.OnEnter();
            int width = Agent.Width;
            int height = Agent.Height;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = Cells[x, y];
                    if (cell.IsVisible)
                    {
                        cell.ClearFlags();
                    }
                }
            }
            Agent.MatchInfos.Clear();
            Agent.BoosterInfo.Clear();
            _finished = true;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_finished) Finished();
        }
    }
}