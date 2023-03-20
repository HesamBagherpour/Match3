using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using HB.Match3.Cells.Modules;

namespace Garage.Match3.View
{
    public interface ICellView
    {
        void  AddModuleView(Point pos, IList<BaseModule> modules);
        void  AddModuleView(Point pos, BaseModule module);
        void Dispose();
    }
}