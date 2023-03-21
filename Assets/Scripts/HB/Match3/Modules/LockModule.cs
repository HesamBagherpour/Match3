using System;
using HB.Match3.Block;
using HB.Match3.Cell;

namespace HB.Match3.Modules
{
    public class LockModule : RestrictionModule
    {
         public BlockColor color;
        private LockModuleView _lockModuleView;

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            if (view is LockModuleView lockModuleView)
                _lockModuleView = lockModuleView;
        }
        public override void Clear(MyCell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            if (damage == 10)
            {
                UnityEngine.Debug.Log($"Removing LockModule from cell {cell.position}");
                Restriction.health = 0;
                id = id.Remove(id.Length - 1);
                id += Restriction.health.ToString();
                _lockModuleView.Clear(cell.position, () =>
                {
                    onFinished?.Invoke(this);
                });
                damage = 0;
            }
            else
            {
                damage = 0;
                onFinished?.Invoke(null);
            }
        }
    }
}