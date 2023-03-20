using System;
using HB.Match3;
using HB.Match3.Cells.Modules;
using MessagePack;

namespace Garage.Match3.Cells.Modules
{

    [MessagePackObject]
    public class VisibleModule : BaseModule
    {
        public const string LayerName = "cell";
        public const int Order = 0;

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