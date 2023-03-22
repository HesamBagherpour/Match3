using HB.Match3.View;
using System;
using System.Collections.Generic;
using DG.Tweening;
using HB.Match3.Cell;
using HB.Match3.Cell.Effect;
using HB.Match3.Modules;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace HB.Match3.Board
{
    public class TilemapLayer : IBoardLayer
    {
        private readonly LayerStack _layerStack;
        private static readonly Dictionary<string, TileBase> IdToTiles = new Dictionary<string, TileBase>();
        private readonly Dictionary<IModuleView, Point> _moduleToPoint = new Dictionary<IModuleView, Point>();
        private readonly UnityEngine.Tilemaps.Tilemap _tileMap;
        private int _width;
        private int _height;
        private Vector2Int _boardOffset;
        private readonly Grid _grid;
        public Transform Grid => _grid.transform;

        public int ActiveCells { get; private set; }

        public TilemapLayer(int sortingOrder, string name, TilemapSettings tilemapSettings, BoardViewData viewData,
            LayerStack layerStack)
        {
            _layerStack = layerStack;
            GameObject tilemapGo = new GameObject(name + "-tilemap")
            {
                layer = BoardView.Match3BoardLayerIndex
            };

            TilemapRenderer tileMapRenderer = tilemapGo.AddComponent<TilemapRenderer>();
            tileMapRenderer.sortingOrder = sortingOrder;
            tileMapRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            _grid = tilemapSettings.grid;
            tilemapGo.transform.SetParent(tilemapSettings.grid.transform, false);
            _tileMap = tilemapGo.GetComponent<UnityEngine.Tilemaps.Tilemap>() ?? tilemapGo.AddComponent<UnityEngine.Tilemaps.Tilemap>();

            if (name == "cell")
            {
                _tileMap.color = new Color(249f / 255f, 240f / 255f, 220f / 255f, 1f);
            }
            AddIdToTiles(viewData);
            Setup(tilemapSettings);
        }

        private static void AddIdToTiles(BoardViewData viewData)
        {
            for (int i = 0; i < viewData.layerViewData.allTiles.Count; i++)
            {
                CellTile tile = viewData.layerViewData.allTiles[i];
                if (!IdToTiles.ContainsKey(tile.name))
                {
                    IdToTiles.Add(tile.name, tile);
                }
            }
        }

        public void SetTile(int x, int y, IModuleView moduleView)
        {
            if (!moduleView.Visible) return;
            Vector3Int position = GetTilePosition(x, y);
            if (IdToTiles.TryGetValue(moduleView.Id, out TileBase tile))
            {
                _tileMap.SetTile(position, tile);
                if (tile.name.Contains("sofa"))
                {
                    var sofaTile = (SofaTile)tile;
                    _tileMap.SetTransformMatrix(position,
                        Matrix4x4.TRS(sofaTile.offsetPosition, Quaternion.identity, sofaTile.scale));

                    _tileMap.RefreshTile(position);
                }
                ActiveCells++;
            }
            else
            {
                _tileMap.SetTile(position, null);
            }

            _moduleToPoint[moduleView] = new Point(x, y);
        }

        public void Setup(TilemapSettings tilemapSettings)
        {
            _width = tilemapSettings.width;
            _height = tilemapSettings.height;
            _boardOffset = tilemapSettings.boardOffset;
        }

        public void Dispose()
        {
            _tileMap.ClearAllTiles();
        }

        private Vector3Int GetTilePosition(int x, int y)
        {
            return new Vector3Int(_boardOffset.x + x, _boardOffset.y + y, 0);
        }

        public TileBase GetTile(int x, int y)
        {
            return _tileMap.GetTile(GetTilePosition(x, y));
        }

        public void Clear(IModuleView module)
        {
            if (_moduleToPoint.ContainsKey(module))
            {
                Point p = _moduleToPoint[module];
                _tileMap.SetTile(GetTilePosition(p.x, p.y), null);
                ActiveCells--;
            }
        }

        public Point GetPosition(IModuleView module)
        {
            if (_moduleToPoint.ContainsKey(module))
            {
                return _moduleToPoint[module];
            }
            return Point.None;
        }

        public Effect PlayEffect(Vector3 pos, string id)
        {
            return _layerStack.PlayEffect(pos, id);
        }

        public Effect PlayEffect(Vector3 pos, Vector3 targetPos, string id)
        {
            return _layerStack.PlayEffect(pos, targetPos, id);
        }
        public virtual bool ReleaseEffect(Effect effect)
        {
            return _layerStack.ReleaseEffect(effect);
        }

        public Vector3 CellToWorld(Point pos)
        {
            return _layerStack.CellToWorld(pos);
        }

        public void Hide()
        {
            var _tileMapColor = _tileMap.color;
            _tileMapColor.a = 0;
            _tileMap.color = _tileMapColor;
        }

        public void Show(Action onComplete)
        {
            if (ActiveCells == 0)
            {
                onComplete?.Invoke();
                return;
            }
            var _tileMapColor = _tileMap.color;
            _tileMapColor.a = 1;
            var colorTween = DOTween.To(() => _tileMap.color, x => _tileMap.color = x, _tileMapColor, 1f).SetOptions(true);
            colorTween.onComplete += () => { onComplete?.Invoke(); };
        }
    }
}