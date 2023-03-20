using UnityEngine;
using UnityEngine.Tilemaps;

namespace Garage.Match3.BoardEditor
{
    [CreateAssetMenu(fileName = "New GameObjectTile", menuName = "Match3/Tiles/GameObject Tile")]
    public class GameObjectTile : CellTile
    {
        // public blockView
        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            tileData.transform = Matrix4x4.identity;
            tileData.sprite = sprite;

            tileData.color = Color.white;
            tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
            tileData.colliderType = Tile.ColliderType.Sprite;
        }
    }
}