using UnityEngine;
using UnityEngine.Tilemaps;

namespace Garage.Match3.BoardEditor
{
    [CreateAssetMenu(fileName = "New BucketTile", menuName = "Match3/Tiles/Bucket Tile")]
    public class BucketTile : RestrictionTile
    {
        public BlockColor color;
        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            tileData.transform = Matrix4x4.identity;
            //tileData.gameObject = gameObject;
            tileData.sprite = sprite;

            tileData.color = Color.white;
            tileData.flags = TileFlags.LockTransform | TileFlags.LockColor;
            tileData.colliderType = Tile.ColliderType.Sprite;
        }
    }
}