using System;
using HB.Core.DI;
using HB.Match3.Cell;

namespace HB.Match3.Modules
{
    public abstract class BaseModule : IModule
    {

        public string type;

        public BaseModule()
        {
            type = GetType().Name;
        }

        public string id;
        public string layerName;
        public int order;


        public IModuleView view { get; private set; }
        public static event Action<BaseModule> Cleared;

        public virtual void SetView(IModuleView view)
        {
            this.view = view;
        }

        public virtual void Init()
        {
        }

        public virtual void OnRegister(IContext context)
        {
        }

        public virtual void Setup(string id)
        {
            this.id = id;
            OnSetup();
        }

        protected abstract void OnSetup();
        public abstract void Clear(MyCell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished);
        protected void InvokeClearEvent(BaseModule module) => Cleared?.Invoke(module);

        public virtual void Dispose()
        {
            view?.Dispose();
        }
    }
}