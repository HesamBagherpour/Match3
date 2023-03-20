using UnityEngine;
using UnityEngine.Tilemaps;

namespace Garage.Match3.BoardEditor
{
    [CreateAssetMenu(fileName = "New Sofa", menuName = "Match3/Tiles/Sofa")]
    public class SofaTile : CellTile
    {
        public Vector2Int size = Vector2Int.one;
        public Vector2 scale = Vector2.one;
        public Vector3 offsetPosition = Vector3.zero;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.flags = TileFlags.None;
            // tileData.transform = Matrix4x4.identity;
            tileData.transform.SetTRS(offsetPosition, Quaternion.identity, scale);
            tileData.sprite = sprite;
            base.GetTileData(position, tilemap, ref tileData);
        }

        // public override void RefreshTile(Vector3Int position, ITilemap tilemap)
        // {
        //     base.RefreshTile(position, tilemap);
        //     SofaTile tile = tilemap.GetTile<SofaTile>(position);
        //     tile.size = size;
        //     tile.scale = scale;
        // }
    }
}