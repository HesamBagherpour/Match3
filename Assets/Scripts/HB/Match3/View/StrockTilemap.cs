using Garage.Match3.BoardEditor;
using HB.Match3;
using HB.Match3.View;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Garage.Match3.View
{
    public class StrockTilemap : BaseCustomTilemap
    {
        private CellTile strockTile;
        private const string tileID = "background-tile-strock";
        private Vector2Int _boardOffset;
        public StrockTilemap(Transform transform, BoardViewData boardViewData)
        {
            strockTile = boardViewData.layerViewData.allTiles.Find(tile => tile.name == tileID);
            var _strockTilemapGo = new GameObject("StrockTilemap")
            {
                layer = BoardView.Match3BoardLayerIndex
            };
            _strockTilemapGo.transform.SetParent(transform, false);
            _tileMap = _strockTilemapGo.AddComponent<Tilemap>();
            _strockTilemapGo.transform.position += new Vector3(0, -0.25f, 0);
            TilemapRenderer tilemapRenderer = _strockTilemapGo.AddComponent<TilemapRenderer>();
            tilemapRenderer.sortingOrder = -1;
            _tileMap.color = new Color(131f / 255f, 82f / 255f, 61f / 255f, 1f);
        }

        public override void SetAllTiles(BoardData boardData)
        {
            _boardOffset = boardData.boardOffset;
            int width = boardData.width;
            int height = boardData.height;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Cells.Cell cell = boardData.cells[i + j * width];
                    if (cell.IsVisible)
                    {
                        var pos = GetTilePosition(i, j);
                        _tileMap.SetTile(pos, strockTile);
                    }
                }
            }
            _tileMap.RefreshAllTiles();
        }

        private Vector3Int GetTilePosition(int x, int y)
        {
            return new Vector3Int(_boardOffset.x + x, _boardOffset.y + y, 0);
        }
    }
}