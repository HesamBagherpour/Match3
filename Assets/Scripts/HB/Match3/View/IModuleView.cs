using System;
using Garage.Match3.Cells;
using HB.Match3.View;

namespace Garage.Match3.View
{
    public interface IModuleView
    {
        string LayerName { get; set; }
        string Id { get; set; }
        int Order { get; set; }
        bool Visible { get; }
        ModuleViewType Type { get; set; }
        IBoardLayer Layer { get; set; }
        void Dispose();
    }
}