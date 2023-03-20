using Garage.Match3.View;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Garage.Match3.BoardEditor
{
    [CreateAssetMenu(fileName = "New FlowerTile", menuName = "Match3/Tiles/Flower Tile")]
    public class FlowerTile : BlockTile
    {
        
        //public Restriction restriction;
        public BlockColor color;
        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            base.GetTileData(location, tileMap, ref tileData);
            if (blockViewData != null) tileData.sprite = blockViewData.sprite;
        }
    }

}