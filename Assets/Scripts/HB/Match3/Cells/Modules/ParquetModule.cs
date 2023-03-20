using System;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.Cells.Modules;
using MessagePack;

namespace Garage.Match3.Cells.Modules
{
    [MessagePackObject]
    public class ParquetModule : BaseModule
    {
        public const string LayerName = "parquet";
        public const int Order = 1;
        private ParquetModuleView parquetModuleView;

        public override void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            onFinished?.Invoke(null);
        }
        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            if (view is ParquetModuleView parquetModuleView)
            {
                this.parquetModuleView = parquetModuleView;
            }
        }

        protected override void OnSetup()
        {
            layerName = LayerName;
            order = Order;
        }

        public void PlayEffect(Point pos)
        {
            parquetModuleView.PlayEffect(pos);
        }

        internal void InvokeQuestEvent() => InvokeClearEvent(this);
    }
}