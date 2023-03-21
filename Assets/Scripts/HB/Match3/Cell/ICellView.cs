using System.Collections.Generic;
using HB.Match3.Modules;

namespace HB.Match3.Cell
{
    public interface ICellView
    {
        void  AddModuleView(Point pos, IList<BaseModule> modules);
        void  AddModuleView(Point pos, BaseModule module);
        void Dispose();
    }
}