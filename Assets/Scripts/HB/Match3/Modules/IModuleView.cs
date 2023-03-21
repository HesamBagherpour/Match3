namespace HB.Match3.Modules
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