using System;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using HB.Match3.Cells.Modules;
using HB.Match3.View;

namespace Garage.Match3.View
{
    public class LockModuleView : RestrictionView
    {
        public LockModuleView(BaseModule module) : base(module)
        {
        }

        internal void Clear(Point pos, Action onFinished)
        {
            var unlockEffect = Layer.PlayEffect(Layer.CellToWorld(pos), "chain-unlock");
            BoardView.PlayAudio("chain-break");
            unlockEffect.OnComplete += () =>
            {
                Layer.Clear(this);
                onFinished?.Invoke();
            };
        }
    }
}