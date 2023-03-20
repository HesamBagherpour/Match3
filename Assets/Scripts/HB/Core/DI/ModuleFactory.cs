namespace Garage.Core.DI
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