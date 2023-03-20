using System;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.Cells.Modules;
using HB.Match3.View;
using MessagePack;
using UnityEngine;

namespace Garage.Match3.Cells.Modules
{
    [MessagePackObject]
    public class CandleModule : RestrictionModule
    {
        [Key(4)] public bool active;
        [IgnoreMember] public bool ReadyToClear { get; private set; }
        private CandleModuleView _candleModuleView;
        private Action<BaseModule> onModuleCleared;

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            ReadyToClear = false;
            if (view is CandleModuleView candleModuleView)
            {
                _candleModuleView = candleModuleView;
                _candleModuleView.SetState(active);
            }
        }
        public override void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            if ((hitType & Restriction.hitType) == hitType)
            {
                active = true;
                _candleModuleView.SetState(active);
                damage = 0;
            }
            if (ReadyToClear)
            {
                onModuleCleared = onFinished;
                _candleModuleView.Clear(OnModuleViewCleared);
            }
            else onFinished?.Invoke(null);
        }

        private void OnModuleViewCleared()
        {
            id = "candle_on";
            onModuleCleared?.Invoke(this);
            Debug.Log("Candle Cleared!" + id);
            InvokeClearEvent(this);
            onModuleCleared = null;
        }

        public void SetReadyToClear()
        {
            ReadyToClear = true;
        }
    }
}