using HB.Match3.Cell;
using System;
namespace HB.Match3.Modules
{
    public class ExitModule : BaseModule
    {
        private ExitModuleView exitModuleView;
        public const string LayerName = "exit-tile";
        public const int Order = 6;

        protected override void OnSetup()
        {
            layerName = LayerName;
            order = Order;
        }

        public override void Clear(MyCell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            exitModuleView.Clear(() => onFinished?.Invoke(this));
            // view.Layer.ReleaseEffect(effect);
        }

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            exitModuleView = (ExitModuleView)view;
            // effect = view.Layer.PlayEffect(pos, "exit-module");
        }
    }
}