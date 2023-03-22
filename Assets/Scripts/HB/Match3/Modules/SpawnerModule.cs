using HB.Match3.Cell;
using System;

namespace HB.Match3.Modules
{
    public class SpawnerModule : BaseModule
    {
        protected override void OnSetup()
        {
            layerName = VisibleModule.LayerName;
            order = VisibleModule.Order;
        }

        public override void Clear(MyCell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            onFinished?.Invoke(null);
        }
    }
}