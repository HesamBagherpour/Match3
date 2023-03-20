using System;
using System.Collections.Generic;

namespace Garage.Core.DI
{
    public class LoadableContext : Context, ILoadable
    {
        public event Action<float> LoadProgress;

        #region Private Fields

        private readonly List<ILoadable> _loadableList;
        private float _loadedModuleCount;
        private float _loadProgress;
        private int _totalLoadable;

        #endregion

        #region  Constructors

        public LoadableContext(IModuleFactory moduleFactory) : base(moduleFactory)
        {
            _loadProgress = 0;
            _loadedModuleCount = 0;
            _loadableList = new List<ILoadable>();
        }

        #endregion

        #region ILoadable Interface

        public void Load(Action<IModule> onLoaded)
        {
            _totalLoadable = _loadableList.Count;
            _loadedModuleCount = 0;
            for (int i = 0; i < _loadableList.Count; i++)
                _loadableList[i].Load(OnModuleLoad + onLoaded);
        }

        #endregion

        #region Public Methods

        public override T Register<T>()
        {
            IModule module = base.Register<T>();
            if (module is ILoadable loadable)
                _loadableList.Add(loadable);

            return (T)module;
        }

        private void OnModuleLoad(IModule module)
        {
            _loadedModuleCount++;
            _loadProgress = _loadedModuleCount / _totalLoadable;
            OnLoadProgress(_loadProgress);
        }

        #endregion

        private void OnLoadProgress(float progress)
        {
            LoadProgress?.Invoke(progress);
        }
    }
}