using System;
using HB.Match3;
using HB.Match3.Cells.Modules;
using MessagePack;

namespace Garage.Match3.Cells.Modules
{
    [MessagePackObject]
    public class SpawnerModule : BaseModule
    {
        protected override void OnSetup()
        {
            layerName = VisibleModule.LayerName;
            order = VisibleModule.Order;
        }

        public override void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            onFinished?.Invoke(null);
        }
    }
}