using System.Collections.Generic;
using HB.Match3;
using HB.Match3.View;
using HB.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Garage.Match3.View
{
    public class MaskTilemap : BaseCustomTilemap
    {
        private readonly Rect spriteRect;
        private readonly Vector2 spritePivot;
        private readonly Vector3 maskScale;
        private readonly ObjectPool<GameObject> maskPool;
        private readonly Sprite maskSprite;
        private Vector2Int _boardOffset;
        private List<GameObject> activeMaskObjects;

        public MaskTilemap(Transform transform)
        {
            activeMaskObjects = new List<GameObject>();
            var _checkerTilemapGo = new GameObject("MaskTilemap")
            {
                layer = BoardView.Match3BoardLayerIndex
            };
            _checkerTilemapGo.transform.SetParent(transform, false);
            _tileMap = _checkerTilemapGo.AddComponent<Tilemap>();
            _checkerTilemapGo.AddComponent<TilemapRenderer>().sortingOrder = 2;
            spriteRect = new Rect(0, 0, 1, 1);
            spritePivot = new Vector2(0.5f, 0.5f);
            maskScale = new Vector3(0.86f, 0.86f, 1);
            maskPool = new ObjectPool<GameObject>(10, CreateMask, GetMask, ReleaseMask);

            Texture2D maskTexture = new Texture2D(1, 1);
            Color empty = Color.white;
            // Color filled = new Color(0, 0, 0, 0);
            maskTexture.SetPixel(0, 0, empty);
            maskTexture.filterMode = FilterMode.Point;
            maskTexture.Apply();

            maskSprite = Sprite.Create(maskTexture, spriteRect, spritePivot, 1);
        }

        private void ReleaseMask(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        private void GetMask(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }

        private GameObject CreateMask()
        {
            var maskObject = new GameObject("Mask-Sprite")
            {
                layer = BoardView.Match3BoardLayerIndex
            };
            maskObject.transform.SetParent(_tileMap.transform, false);
            maskObject.transform.localScale = maskScale;
            var spriteMask = maskObject.AddComponent<SpriteMask>();
            spriteMask.sprite = maskSprite;
            maskObject.gameObject.SetActive(false);
            return maskObject;
        }

        public override void Clear()
        {
            foreach (var maskObject in activeMaskObjects)
            {
                maskPool.Release(maskObject);
            }
            activeMaskObjects.Clear();
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
                    Cell cell = boardData.cells[i + j * width];
                    if (cell.IsVisible == false)
                    {
                        var pos = GetTilePosition(i, j);
                        Vector3 worldPos = _tileMap.GetCellCenterWorld(pos);
                        CreateMaskSprite(worldPos);
                    }
                }
            }
        }

        private Vector3Int GetTilePosition(int x, int y)
        {
            return new Vector3Int(_boardOffset.x + x, _boardOffset.y + y, 0);
        }

        private void CreateMaskSprite(Vector3 worldPos)
        {
            var maskObject = maskPool.Get();
            maskObject.transform.position = worldPos;
            activeMaskObjects.Add(maskObject);
        }
    }
}