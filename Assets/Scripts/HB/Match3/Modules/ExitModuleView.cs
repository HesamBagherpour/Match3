using HB.Match3.View;
using System;
using UnityEngine;

namespace HB.Match3.Modules
{
    public class ExitModuleView : ObjectModuleView
    {
        public ExitModuleView(BaseModule module) : base(module) { }

        public override void Clear(Action onFinished)
        {
            Layer.Clear(this);
            onFinished?.Invoke();
        }

        public override void SetGameObject(GameObject go) { }
    }
}