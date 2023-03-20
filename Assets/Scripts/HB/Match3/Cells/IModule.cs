using Garage.Core.DI;

namespace HB.Match3.Cells
{
    public interface IModule
    {
        #region Public Methods

        void OnRegister(IContext context);
        void Init();

        #endregion

    }
}