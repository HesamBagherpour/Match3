using Garage.Match3.Cells.Modules;
using HB.Match3.Cells.Modules;
using HB.Match3.View;

namespace Garage.Match3.View
{
    public class VisibleModuleView : ModuleView
    {
        public VisibleModuleView(BaseModule module) : base(module)
        {
            Visible = true;
        }
    }

    public class EmptyModuleView : ModuleView
    {
        public EmptyModuleView(BaseModule module) : base(module)
        {
            Visible = false;
        }
    }
}