using HB.Match3;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Garage.Match3.BoardEditor
{
    [CreateAssetMenu(fileName = "New CellTile", menuName = "Match3/Tiles/Cell Tile")]
    public class CellTile : TileBase
    {
        public Restriction restriction;

        [SerializeField] public Sprite sprite;
        [SerializeField] public Sprite questSprite;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            base.GetTileData(position, tilemap, ref tileData);
            tileData.sprite = sprite;
        }
    }
}