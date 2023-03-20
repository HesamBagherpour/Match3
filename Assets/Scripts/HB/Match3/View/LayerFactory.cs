using System;
using HB.Match3.View;

namespace Garage.Match3.View
{
    public static class LayerFactory
    {
        public static IBoardLayer Create(IModuleView moduleView, TilemapSettings settings, BoardViewData data,
            LayerStack layerStack)
        {
            IBoardLayer layer;
            switch (moduleView.Type)
            {
                case ModuleViewType.Tile:
                    layer = new TilemapLayer(
                        moduleView.Order,
                        moduleView.LayerName,
                        settings,
                        data,
                        layerStack);

                    break;
                case ModuleViewType.GameObject:

                    if (moduleView is BlockModuleView)
                    {
                        layer = new BlockObjectLayer(moduleView.Order,
                            moduleView.LayerName,
                            settings,
                            data,
                            layerStack);
                    }
                    else
                    {
                        layer = new ObjectLayer(
                            moduleView.Order,
                            moduleView.LayerName,
                            settings,
                            data,
                            layerStack);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            moduleView.Layer = layer;
            return layer;
        }
    }
}