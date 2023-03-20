using System;
using HB.Core.DI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Garage.Core.DI
{
    public class ImplicitModuleFactory : IModuleFactory
    {
        #region Private Fields

        private readonly GameObject _gameObject;
        private readonly bool _persistence;

        #endregion

        #region  Constructors

        public ImplicitModuleFactory(GameObject gameObject, bool persistence)
        {
            _gameObject = gameObject;
            _persistence = persistence;
            if (_persistence)
            {
                Object.DontDestroyOnLoad(_gameObject);
            }

        }

        #endregion

        #region IModuleFactory Interface

        public IModule Create<T>() where T : IModule, new()
        {
            Type type = typeof(T);
            return type.BaseType == typeof(MonoModule) ? (IModule) CreateMonoModule(type) : new T();
        }

        #endregion

        #region Private Methods

        private MonoModule CreateMonoModule(Type componentType)
        {
            Component m = _gameObject.AddComponent(componentType);
            if (_persistence)
            {
                Object.DontDestroyOnLoad(m);
            }

            return (MonoModule) m;
        }

        #endregion
    }
}