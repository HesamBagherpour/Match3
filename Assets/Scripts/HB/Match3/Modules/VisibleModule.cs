using System;
using HB.Match3.Cell;

namespace HB.Match3.Modules
{
    public class VisibleModule : BaseModule
    {
        public const string LayerName = "cell";
        public const int Order = 0;

        protected override void OnSetup()
        {
            layerName = LayerName;
            order = Order;
        }

        public override void Clear(MyCell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            onFinished?.Invoke(null);
        }
    }
}