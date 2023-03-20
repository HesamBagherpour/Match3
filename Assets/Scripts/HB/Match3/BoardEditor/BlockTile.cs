using Garage.Match3.View;
using HB.Match3.View;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Garage.Match3.BoardEditor
{
    [CreateAssetMenu(fileName = "New BlockTile", menuName = "Tiles/Block Tile")]
    public class BlockTile : GameObjectTile
    {
        public BlockViewData blockViewData;

        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            base.GetTileData(location, tileMap, ref tileData);
            if (blockViewData != null) tileData.sprite = blockViewData.sprite;
        }
    }

}