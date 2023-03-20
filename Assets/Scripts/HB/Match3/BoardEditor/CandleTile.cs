using UnityEngine;
using UnityEngine.Tilemaps;

namespace Garage.Match3.BoardEditor
{
    [CreateAssetMenu(fileName = "New CandleTile", menuName = "Match3/Tiles/Candle Tile")]
    public class CandleTile : RestrictionTile
    {
        public bool candleIsOn;
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