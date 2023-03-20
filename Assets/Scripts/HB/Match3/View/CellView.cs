using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3.Cells.Modules;

namespace HB.Match3.View
{
    public class CellView : ICellView
    {
        private readonly LayerStack _layerStack;
        private readonly BoardViewData _boardViewData;

        public CellView(LayerStack layerStack, BoardViewData boardViewData)
        {
            _layerStack = layerStack;
            _boardViewData = boardViewData;
        }

        public void AddModuleView(Point pos, IList<BaseModule> modules)
        {
            for (int i = 0; i < modules.Count; i++)
            {
                BaseModule module = modules[i];
                if (module.view == null) AddModuleView(pos, module);
            }
        }


        public void AddModuleView(Point pos, BaseModule module)
        {
            IModuleView moduleView = ModuleViewFactory.Create(module, _boardViewData);
            _layerStack.SetTile(moduleView, pos.x, pos.y);
            moduleView.Layer = _layerStack.GetLayer(moduleView.LayerName);
            module.SetView(moduleView);
        }

        public void Dispose()
        {
        }
    }
}