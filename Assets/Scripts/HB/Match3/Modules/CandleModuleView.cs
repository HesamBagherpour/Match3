using System;
using HB.Match3.Board;
using HB.Match3.View;
using UnityEngine;

namespace HB.Match3.Modules
{
    public class CandleModuleView : ObjectModuleView
    {
        CandleView _candleView;
        private bool isOn;

        public CandleModuleView(BaseModule module) : base(module)
        {
            Visible = true;
        }

        public override void Clear(Action onFinished)
        {
            Layer.Clear(this);
            var effect = Layer.PlayEffect(_candleView.transform.position, "clear-candle");
            effect.OnClear += () =>
            {
                onFinished?.Invoke();
            };
        }

        public override void SetGameObject(GameObject go)
        {
            _candleView = go.GetComponent<CandleView>();
        }
        internal void SetState(bool state)
        {
            if (isOn == false && state == true)
                BoardView.PlayAudio("candle-on");
            isOn = state;
            _candleView.SetState(state);
            // if (state) Layer.PlayEffect(_candleView.transform.position, "candle-active");
        }
    }
    
    
}