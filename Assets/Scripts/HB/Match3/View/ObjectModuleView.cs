using System;
using HB.Match3.Modules;
using UnityEngine;

namespace HB.Match3.View
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