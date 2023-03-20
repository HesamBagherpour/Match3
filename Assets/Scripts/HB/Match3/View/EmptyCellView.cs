using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using HB.Match3.Cells.Modules;

namespace Garage.Match3.View
{
    public class EmptyCellView : ICellView
    {
        public IModuleView GetModuleView(BaseModule module)
        {
            return new EmptyModuleView(module);
        }

        public void RemoveModuleView(BaseModule module)
        {
        }

        public void AddModuleView(Point pos, BaseModule module)
        {
            module.SetView(new EmptyModuleView(module));
        }

        public void AddModuleView(Point pos, IList<BaseModule> modules)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                AddModuleView(pos, modules[i]);
            }
        }

        public void Dispose()
        {
        }
    }
}