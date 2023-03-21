using HB.Core.DI;

namespace HB.Match3.Modules
{
    public class ModuleFactory : IModuleFactory
    {
        #region IModuleFactory<Module> Interface

        public IModule Create<T>() where T : IModule, new() 
        {
            T t = new T();
            return t;
        }

        #endregion
    }
}