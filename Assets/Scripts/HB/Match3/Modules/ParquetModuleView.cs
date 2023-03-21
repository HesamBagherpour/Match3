using HB.Match3.Cell;

namespace HB.Match3.Modules
{
    public class ParquetModuleView : ModuleView
    {
        public ParquetModuleView(BaseModule module) : base(module)
        {
            Visible = true;
        }
        public void PlayEffect(Point pos)
        {
            Layer.PlayEffect(Layer.CellToWorld(pos), "parquet");
        }
    }
}