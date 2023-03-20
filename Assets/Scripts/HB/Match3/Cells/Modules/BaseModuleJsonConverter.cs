using System;
using System.Collections.Generic;
using DefaultNamespace;
using HB.Match3.BoardModules;
using HB.Match3.Cells.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Garage.Match3.Cells.Modules
{
    public class BaseModuleJsonConverter : JsonCreationConverter<BaseModule>
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }


        protected override BaseModule Create(Type objectType, JObject jObject)
        {
            Dictionary<string, BaseModule> _modules = new Dictionary<string, BaseModule>()
            {
                {typeof(RestrictionModule).Name, new RestrictionModule()},
                {typeof(BlockModule).Name, new BlockModule()},
                {typeof(VisibleModule).Name, new VisibleModule()},
                {typeof(SpawnerModule).Name, new SpawnerModule()},
                {typeof(InvisibleBlocker).Name, new InvisibleBlocker()},
                {typeof(GlassModule).Name, new GlassModule()},
                {typeof(WoodIronModule).Name, new WoodIronModule()},
                {typeof(GrassModule).Name, new GrassModule()},
                {typeof(SofaModule).Name, new SofaModule()},
                {typeof(ExitModule).Name, new ExitModule()},
                {typeof(CannonModule).Name, new CannonModule()},
                {typeof(IronWallModule).Name, new IronWallModule()},
                {typeof(BucketModule).Name, new BucketModule()},
                {typeof(BoosterModule).Name, new BoosterModule()},
                {typeof(CandleModule).Name, new CandleModule()},
                {typeof(LockModule).Name, new LockModule()},
                {typeof(LockQuestModule).Name, new LockQuestModule()},
                {typeof(ParquetModule).Name, new ParquetModule()},
                {typeof(MeterSpawnerModule).Name, new MeterSpawnerModule()},
                {typeof(FlowerModule).Name, new FlowerModule()}
            };
            BaseModule targetModule = null;
            foreach (var keyValuePair in _modules)
            {
                if (jObject["type"].Value<string>() == keyValuePair.Key)
                {
                    targetModule = keyValuePair.Value;
                    break;
                }
            }

            if (targetModule == null)
                throw new Exception(
                    $"[custom json formatter][basemodule] Target module: {jObject["type"].Value<string>()} not found");
            return targetModule;
        }
    }
}