using Garage.Core.DI;
using UnityEngine;
using IModule = HB.Match3.Cells.IModule;

namespace HB.Core.DI
{
    public abstract class MonoModule : MonoBehaviour, IModule
    {
        #region IModule Interface



        public virtual void OnRegister(IContext context)
        {
          
        }

        public virtual void Init()
        {
        }

        public override string ToString()
        {
            return GetType().Name;
        }

        #endregion
    }
}