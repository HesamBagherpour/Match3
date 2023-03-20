using System;

namespace Garage.Core.DI
{
    public interface ILoadable
    {
        #region Public Methods

        void Load(Action<IModule> onLoaded);

        #endregion
    }
}