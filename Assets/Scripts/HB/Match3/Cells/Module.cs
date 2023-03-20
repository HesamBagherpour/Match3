using Garage.Core.DI;

namespace HB.Match3.Cells
{
    public abstract class Module : IModule
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