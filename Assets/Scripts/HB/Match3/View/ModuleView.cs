using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3.Cells.Modules;

namespace HB.Match3.View
{
    public abstract class ModuleView : IModuleView
    {
        #region Public Properties

        public IBoardLayer Layer { get; set; }

        public string LayerName { get; set; }
        public string Id { get; set; }
        public int Order { get; set; }
        public ModuleViewType Type { get; set; }
        public bool Visible { get; protected set; }

        #endregion

        #region  Constructors

        protected ModuleView(BaseModule module)
        {
            SetModuleData(module);
        }

        public void SetModuleData(BaseModule module)
        {
            Id = module.id;
            LayerName = module.layerName;
            Order = module.order;
        }

        #endregion

        #region IModuleView Interface

        public virtual void Dispose()
        {
            Layer.Clear(this);
        }

        #endregion
    }
}