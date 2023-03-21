using System;
using HB.Match3.Cell;
using UnityEngine;

namespace HB.Match3.Modules
{
    public class CandleModule : RestrictionModule
    {
        public bool active;
        public bool ReadyToClear { get; private set; }
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
        public override void Clear(MyCell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
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