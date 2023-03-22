namespace HB.Match3.Modules
{
    public class InvisibleBlocker : RestrictionModule
    {
        protected override void OnSetup()
        {
            layerName = VisibleModule.LayerName;
            order = VisibleModule.Order;
        }
    }
}