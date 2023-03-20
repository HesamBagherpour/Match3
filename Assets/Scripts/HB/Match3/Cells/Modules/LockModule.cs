using System;
using Garage.Match3;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using MessagePack;

namespace HB.Match3.Cells.Modules
{
    [MessagePackObject]
    public class LockModule : RestrictionModule
    {
        [Key(4)] public BlockColor color;
        private LockModuleView _lockModuleView;

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            if (view is LockModuleView lockModuleView)
                _lockModuleView = lockModuleView;
        }
        public override void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
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