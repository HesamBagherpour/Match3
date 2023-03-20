using System;
using Garage.Core.DI;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3.BoardModules;
using MessagePack;

namespace HB.Match3.Cells.Modules
{
    [MessagePackObject]
    [Union(0, typeof(RestrictionModule))]
    [Union(1, typeof(BlockModule))]
    [Union(2, typeof(VisibleModule))]
    [Union(3, typeof(SpawnerModule))]
    [Union(4, typeof(InvisibleBlocker))]
    [Union(5, typeof(GlassModule))]
    [Union(6, typeof(WoodIronModule))]
    [Union(7, typeof(GrassModule))]
    [Union(8, typeof(SofaModule))]
    [Union(9, typeof(ExitModule))]
    [Union(10, typeof(CannonModule))]
    [Union(11, typeof(IronWallModule))]
    [Union(12, typeof(BucketModule))]
    [Union(13, typeof(BoosterModule))]
    [Union(14, typeof(CandleModule))]
    [Union(15, typeof(LockModule))]
    [Union(16, typeof(LockQuestModule))]
    [Union(17, typeof(ParquetModule))]
    [Union(18, typeof(MeterSpawnerModule))]
    [Union(19, typeof(FlowerModule))]
    public abstract class BaseModule : Garage.Core.DI.IModule
    {
        [IgnoreMember] public string type;

        public BaseModule()
        {
            type = GetType().Name;
        }

        [Key(0)] public string id;
        [Key(1)] public string layerName;
        [Key(2)] public int order;


        [IgnoreMember] public IModuleView view { get; private set; }
        public static event Action<BaseModule> Cleared;

        public virtual void SetView(IModuleView view)
        {
            this.view = view;
        }

        public virtual void Init()
        {
        }

        public virtual void OnRegister(IContext context)
        {
        }

        public virtual void Setup(string id)
        {
            this.id = id;
            OnSetup();
        }

        protected abstract void OnSetup();
        public abstract void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished);
        protected void InvokeClearEvent(BaseModule module) => Cleared?.Invoke(module);

        public virtual void Dispose()
        {
            view?.Dispose();
        }
    }
}