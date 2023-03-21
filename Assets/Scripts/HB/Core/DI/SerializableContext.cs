using System;
using System.Collections.Generic;
using HB.Match3.Modules;
using HB.Match3.Cell;
using System.Linq;

namespace HB.Core.DI
{
    public class SerializableContext : Context
    {
        
        public Dictionary<string, BaseModule> serializedModules = new Dictionary<string, BaseModule>();

        public SerializableContext(IModuleFactory moduleFactory) : base(moduleFactory)
        {
        }

        public SerializableContext() : base(new ModuleFactory())
        {
        }

        public void Serialize()
        {
            serializedModules.Clear();
            foreach (KeyValuePair<Type, IModule> module in Modules)
            {
                string typeName = module.Key.FullName ?? throw new InvalidOperationException();
                if (serializedModules.ContainsKey(typeName) == false)
                    serializedModules.Add(typeName, (BaseModule)module.Value);
            }
        }

        public void Deserialize()
        {
            Modules.Clear();
            foreach (KeyValuePair<string, BaseModule> module in serializedModules)
            {
                Modules.Add(module.Value.GetType(), module.Value);
            }
        }
    }
}