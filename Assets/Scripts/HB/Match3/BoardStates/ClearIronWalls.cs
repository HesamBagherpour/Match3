using Garage.Match3.Cells.Modules;
using HB.Match3;
using HB.Match3.Cells.Modules;

namespace Garage.Match3.BoardStates
{
    public class ClearIronWalls : BoardState
    {
        private bool _finished;
        private int _counter;

        protected override void OnEnter()
        {
            base.OnEnter();
            _counter = 0;
            foreach (var cell in Agent.Cells)
            {
                // Hit IronWalls and clear if needed
                var ironWallModule = cell.GetModule<IronWallModule>();
                if (ironWallModule != null)
                {
                    int dmg = 1;
                    _counter++;
                    _finished = false;
                    ironWallModule.Clear(cell, ref dmg, HitType.Direct, OnIronWallCleared);
                    if (ironWallModule.Restriction.health == 0)
                    {
                        cell.RemoveModule<IronWallModule>();
                    }
                }
            }
            if (_counter == 0)
            {
                _finished = true;
            }
        }

        private void OnIronWallCleared(BaseModule module)
        {
            _counter--;
            if (_counter == 0)
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
    }
}