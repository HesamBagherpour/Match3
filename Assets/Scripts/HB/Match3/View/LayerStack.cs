using System;
using System.Collections.Generic;
using HB.Match3.Board;
using HB.Match3.Cell;
using HB.Packages.Logger;
using HB.Match3.Cell.Effect;
using HB.Packages.Utilities;
using HB.Match3.Modules;
using HB.Match3.Tilemap;
using HB.Packages.Logger;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HB.Match3.View
{
    public class LayerStack
    {
        private BoardViewData _viewData;
        private TilemapSettings _tilemapSettings;
        private readonly Dictionary<string, IBoardLayer> _layers;
        private static readonly Dictionary<string, ObjectPool<Effect>> Effects =
            new Dictionary<string, ObjectPool<Effect>>();

        private readonly ICustomTilemap _checker;
        private readonly ICustomTilemap _mask;
        private readonly ICustomTilemap _strock;
        private int tilemapLayerCounter;
        private int objectLayerCounter;
        private Action _showComplete;

        public LayerStack(TilemapSettings tilemapSettings, BoardViewData viewData)
        {
            _tilemapSettings = tilemapSettings;
            _layers = new Dictionary<string, IBoardLayer>();
            _viewData = viewData;
            List<Effect> effects = _viewData.layerViewData.effects;
            for (int i = 0; i < effects.Count; i++)
            {
                if (Effects.ContainsKey(effects[i].id) == false)
                    Effects.Add(effects[i].id, CreatePool(effects[i]));
            }
            Transform gridTransform = _tilemapSettings.grid.transform;
            //HB
            // _checker = new CheckerTilemap(gridTransform, viewData);
            // _mask = new MaskTilemap(gridTransform);
            // _strock = new StrockTilemap(gridTransform, viewData);
        }

        public void UpdateTilemapSetting(TilemapSettings tilemapSettings)
        {
            _tilemapSettings = tilemapSettings;
            foreach (var item in _layers)
            {
                item.Value.Setup(_tilemapSettings);
            }
        }

        private ObjectPool<Effect> CreatePool(Effect prefab)
        {
            ObjectPool<Effect> pool =
                new ObjectPool<Effect>(20,
                    () =>
                    {
                        Effect effect = Object.Instantiate(prefab);
                        effect.gameObject.transform.SetParent(_tilemapSettings.grid.transform, false);
                        effect.gameObject.SetActive(false);
                        return effect;
                    },
                    effect => effect.gameObject.SetActive(true),
                    effect =>
                    {
                        effect.transform.SetParent(_tilemapSettings.grid.transform, false);
                        effect.gameObject.SetActive(false);
                    });

            pool.WarmUp(prefab.poolWarmUpCount);
            return pool;
        }


        public void SetTile(IModuleView moduleView, int x, int y)
        {
            if (_layers.TryGetValue(moduleView.LayerName, out IBoardLayer layer) == false)
            {
                ////HBlayer = LayerFactory.Create(moduleView, _tilemapSettings, _viewData, this);//HB
                _layers.Add(moduleView.LayerName, layer);
            }

            layer?.SetTile(x, y, moduleView);
        }

        public IBoardLayer GetLayer(string layerName)
        {
            if (_layers.ContainsKey(layerName) == false)
            {
                Debug.LogError(layerName);
            }
            return _layers[layerName];
        }

        public Vector3 CellToWorld(Point point)
        {
            Grid grid = _tilemapSettings.grid;
            Vector2Int boardOffset = _tilemapSettings.boardOffset;
            return grid.GetCellCenterWorld(new Vector3Int(boardOffset.x + point.x, boardOffset.y + point.y, -1));
        }

        public bool ReleaseEffect(Effect effect)
        {
            if (Effects.TryGetValue(effect.id, out var pool))
            {
                pool.Release(effect);
                return true;
            }
            return false;
        }

        public Effect PlayEffect(Vector3 pos, Vector3 targetPos, string id)
        {
            if (Effects.TryGetValue(id, out var pool))
            {
                Effect effect = pool.Get();
                effect.transform.position = pos;
                effect.Play(targetPos);
                effect.OnComplete += () => pool.Release(effect);
                return effect;
            }
            Log.Error("Match3", $"Could not find {id} effect at {pos} cell");
            return null;
        }

        internal void UpdateCheckerAndMask(BoardData boardData)
        {
            //HB
            // _checker.SetAllTiles(boardData);
            // _mask.SetAllTiles(boardData);
            // _strock.SetAllTiles(boardData);
            //HB
        }

        public Effect PlayEffect(Vector3 pos, string id)
        {
            if (Effects.TryGetValue(id, out ObjectPool<Effect> pool))
            {
                Effect effect = pool.Get();
                effect.transform.position = pos;
                effect.Play();
                effect.OnComplete += () =>
                {
                    pool.Release(effect);
                    effect.RemoveAllListeners();
                };
                return effect;
            }
            Log.Error("Match3", $"Could not find {id} effect at {pos} cell");
            return null;
        }

        internal void Hide()
        {
            foreach (var layer in _layers)
            {
                layer.Value.Hide();
            }
            _mask.Hide();
            _strock.Hide();
            _checker.Hide();
        }

        internal void Clear()
        {
            _checker?.Clear();
            _mask?.Clear();
            _strock?.Clear();
            foreach (KeyValuePair<string, ObjectPool<Effect>> effectPair in Effects)
            {
                effectPair.Value.ReleaseAll();
            }
        }

        internal void Show(Action onComplete)
        {
            tilemapLayerCounter = 0;
            objectLayerCounter = 0;
            _showComplete = onComplete;
            foreach (var layer in _layers)
            {
                //HB
                // if (layer.Value is TilemapLayer tilemapLayer)
                // {
                //     tilemapLayerCounter++;
                //     tilemapLayer.Show(OnTilemapLayerShowComplete);
                // }
                //HB
                _mask.Show(null);
                _strock.Show(null);
                _checker.Show(null);
            }
            if (tilemapLayerCounter == 0) LayerStackShowComplete();
        }

        private void OnTilemapLayerShowComplete()
        {
            tilemapLayerCounter--;
            if (tilemapLayerCounter == 0)
            {
                //HBBoardView.PlayAudio("BoardStart", 0.2f);//HB
                foreach (var layer in _layers)
                {
                    //HB
                    // if (layer.Value is ObjectLayer objectLayer)
                    // {
                    //     objectLayerCounter++;
                    //     objectLayer.Show(OnObjectLayerShowComplete);
                    // }
                    //HB
                }
                if (objectLayerCounter == 0) LayerStackShowComplete();
            }
        }

        private void OnObjectLayerShowComplete()
        {
            objectLayerCounter--;
            if (objectLayerCounter == 0)
            {
                LayerStackShowComplete();
            }
        }

        private void LayerStackShowComplete()
        {
            _showComplete?.Invoke();
        }
    }
}