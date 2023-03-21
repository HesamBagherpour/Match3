using HB.Match3.Block;

namespace HB.Match3.Modules
{
    public class FlowerModule :RestrictionModule
    {
        
        public BlockColor color;
         public bool IsActive { get; private set; }
        private FlowerModuleView _flowerModuleView;

    }
}