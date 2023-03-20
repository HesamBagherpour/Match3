using Garage.Match3.BoardEditor;
using Garage.Match3.View;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HB.Match3.View
{
    public class CheckerTilemap : BaseCustomTilemap
    {
        private Vector2Int _boardOffset;
        private readonly CellTile checkerTile;
        private const string tileID = "bachground-tile-gray";

        public CheckerTilemap(Transform transform, BoardViewData boardViewData)
        {
            checkerTile = boardViewData.layerViewData.allTiles.Find(tile => tile.name == tileID);
            var _checkerTilemapGo = new GameObject("CheckerTilemap")
            {
                layer = BoardView.Match3BoardLayerIndex
            };
            _checkerTilemapGo.transform.SetParent(transform, false);
            _tileMap = _checkerTilemapGo.AddComponent<Tilemap>();
            _checkerTilemapGo.AddComponent<TilemapRenderer>().sortingOrder = 1;
        }

        public override void SetAllTiles(BoardData boardData)
        {
            _boardOffset = boardData.boardOffset;
            int width = boardData.width;
            int height = boardData.height;
            Color gray = Color.gray;
            gray.a = 0.2f;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        Garage.Match3.Cells.Cell cell = boardData.cells[i + j * width];
                        if (cell.IsVisible)
                        {
                            var pos = GetTilePosition(i, j);
                            _tileMap.SetTile(pos, checkerTile);
                            _tileMap.SetColor(pos, gray);
                            //Debug.Log($"Painting checker at {i},{j} =>{pos}");
                        }
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