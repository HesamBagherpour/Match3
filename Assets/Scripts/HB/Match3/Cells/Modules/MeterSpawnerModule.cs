using System;
using HB.Match3;
using HB.Match3.Cells.Modules;
using MessagePack;

namespace Garage.Match3.Cells.Modules
{
    [MessagePackObject]
    public class MeterSpawnerModule : BaseModule
    {
        public const string LayerName = "meter";
        public const int Order = 10;
        protected override void OnSetup()
        {
            layerName = LayerName;
            order = Order;
        }

        public override void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            onFinished?.Invoke(null);
        }
    }
}