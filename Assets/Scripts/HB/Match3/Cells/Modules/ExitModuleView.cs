using System;
using Garage.Match3.View;
using HB.Match3.Cells.Modules;
using UnityEngine;

namespace Garage.Match3.Cells.Modules
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