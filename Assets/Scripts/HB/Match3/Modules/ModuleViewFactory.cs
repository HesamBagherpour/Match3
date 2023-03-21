

using HB.Match3.View;

namespace HB.Match3.Modules
{
    public static class ModuleViewFactory
    {
        public static IModuleView Create(BaseModule module, BoardViewData boardViewData)
        {
            switch (module)
            {
                // case BoosterModule boosterModule:
                //     return new BoosterModuleView(boosterModule);
                // case BucketModule bucketModule:
                //     return new BucketModuleView(bucketModule);
                // case FlowerModule FlowerModule:
                //     return new FlowerModuleView(FlowerModule);
                // case CandleModule candleModule:
                //     return new CandleModuleView(candleModule);
                // case LockQuestModule lockQuestModule:
                //     return new LockModuleQuestView(lockQuestModule);
                // case LockModule lockModule:
                //     return new LockModuleView(lockModule);
                // case RestrictionModule restrictionModule:
                //     return new RestrictionView(restrictionModule);
                // case IronWallModule ironWallModule:
                //     return new RestrictionView(ironWallModule);
                // case BlockModule blockModule:
                //     return new BlockModuleView(blockModule, boardViewData.blockViewData);
                // case VisibleModule visibleModule:
                //     return new VisibleModuleView(visibleModule);
                // case ParquetModule parquetModule:
                //     return new ParquetModuleView(parquetModule);
                // case ExitModule exitModule:
                //     return new ExitModuleView(exitModule);
                // case SofaModule sofa:
                //     return new SofaView(sofa);
                case CannonModule cannonModule:
                    return new CannonModuleView(cannonModule);
            }

            return new EmptyModuleView(module);
        }
    }
}
