namespace Garage.Core.DI
{
    public interface IModule
    {
        #region Public Methods

        void OnRegister(IContext context);
        void Init();

        #endregion

    }
}