using HB.Core.DI;

namespace HB.Match3.Modules
{
    public interface IModuleFactory
    {
        #region Public Methods

        IModule Create<T>() where T : IModule, new();

        #endregion
    }
}