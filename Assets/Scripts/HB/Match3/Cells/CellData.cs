
using Garage.Match3.Cells;
using MessagePack;

namespace HB.Match3.Cells
{
    [MessagePackObject]
    public class CellData
    {
        [Key(1)] public Point position;
        [Key(2)] public SerializableContext context;
    }
}