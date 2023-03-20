using System;
using Garage.Core.DI;
using UnityEngine;

namespace HB.Core.DI
{
    public class MonoModuleFactory<TModule> : IModuleFactory where TModule : MonoModule
    {
        #region Private Fields

        private readonly GameObject _gameObject;

        #endregion

        #region  Constructors

        public MonoModuleFactory(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        #endregion

        #region IModuleFactory Interface

        public IModule Create<T>() where T : IModule, new()
        {
            T t = default;
            return t is TModule ? CreateMonoModule(typeof(T)) : default(IModule);
        }

        #endregion

        #region Private Methods

        private MonoModule CreateMonoModule(Type componentType)
        {
            Component m = _gameObject.AddComponent(componentType);
            return (MonoModule) m;
        }

        #endregion
    }
}