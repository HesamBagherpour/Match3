using System;
using Garage.Match3.Cells.Modules;
using HB.Match3.Cells.Modules;
using HB.Match3.View;
using UnityEngine;

namespace Garage.Match3.View
{
    public abstract class ObjectModuleView : ModuleView
    {
        public abstract void SetGameObject(GameObject go);

        protected ObjectModuleView(BaseModule module) : base(module)
        {
            Visible = true;
            Type = ModuleViewType.GameObject;
        }

        public abstract void Clear(Action onFinished);
    }
}