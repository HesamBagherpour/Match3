using Garage.Match3;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Garage.Match3.View.BlockView;

namespace HB.Match3.BoardEditor
{
    [CreateAssetMenu(fileName = "New LockTile", menuName = "Match3/Tiles/Lock Tile")]
    public class LockTile : TerrainTile
    {
        public BlockColor blockColor;
        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.flags = TileFlags.LockAll;
            switch (blockColor)
            {
                case BlockColor.Red:
                    tileData.color = ColorSetting.brightRed;
                    break;
                case BlockColor.Green:
                    tileData.color = ColorSetting.brightGreen;
                    break;
                case BlockColor.Cyan:
                    tileData.color = ColorSetting.brightCyan;
                    break;
                case BlockColor.Purple:
                    tileData.color = ColorSetting.brightPurple;
                    break;
                case BlockColor.Yellow:
                    tileData.color = ColorSetting.brightYellow;
                    break;
                case BlockColor.Orange:
                    tileData.color = ColorSetting.brightOrange;
                    break;
                case BlockColor.None:
                    tileData.color = Color.white;
                    break;
                default:
                    break;
            }
        }
    }
}