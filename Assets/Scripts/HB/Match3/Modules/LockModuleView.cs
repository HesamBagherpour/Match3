using System;
using HB.Match3.Board;
using HB.Match3.Cell;

namespace HB.Match3.Modules
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