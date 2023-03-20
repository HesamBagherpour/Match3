using System.Collections.Generic;

namespace Garage.Core.DI
{
    public interface IContext
    {
        #region Public Methods

        T Register<T>() where T : IModule, new();
        T Get<T>() where T : class, IModule;
        T Remove<T>() where T : class, IModule;
        void Init();

        #endregion

        IList<T> GetAll<T>();
        void AddInstance(IModule module);
        void RemoveInstance(IModule module);
    }
}