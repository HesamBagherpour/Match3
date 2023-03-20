using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using HB.Match3.Cells.Modules;
using HB.Match3.View;

namespace Garage.Match3.View
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