using System;
using Garage.Match3.View;
using HB.Match3.View;
#if UNITY_EDITOR
using UnityEditor.Tilemaps;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Match3.BoardEditor
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    [RequireComponent(typeof(HB.Match3.BoardEditor.BoardEditor))]
    public class TilePalletteHelper : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] HB.Match3.BoardEditor.BoardEditor boardEditor;
        private string currentPalette;

        private void Awake()
        {
            if (boardEditor == null) boardEditor = GetComponent<HB.Match3.BoardEditor.BoardEditor>();
        }

        private void Update()
        {
            if (GridPaintingState.palette != null && currentPalette != GridPaintingState.palette.name)
            {
                currentPalette = GridPaintingState.palette.name.ToLower();
                ChangePalette();
            }
        }

        private void ChangePalette()
        {
            if (currentPalette.Contains("cell"))
            {
                GridPaintingState.scenePaintTarget = GameObject.Find("cell");
            }
            else if (currentPalette.Contains("block"))
            {
                GridPaintingState.scenePaintTarget = GameObject.Find("blocks");
            }
            else if (currentPalette.Contains("sofa"))
            {
                GridPaintingState.scenePaintTarget = GameObject.Find("sofa");
            }
            else if (currentPalette.Contains("glass"))
            {
                GridPaintingState.scenePaintTarget = GameObject.Find("glass");
            }
            else if (currentPalette.Contains("wood"))
            {
                GridPaintingState.scenePaintTarget = GameObject.Find("wood-iron");
            }
            else if (currentPalette.Contains("grass"))
            {
                GridPaintingState.scenePaintTarget = GameObject.Find("grass");
            }
            else if (currentPalette.Contains("tutorial"))
            {
                GridPaintingState.scenePaintTarget = GameObject.Find("tutorial");
            }
            else if (currentPalette.Contains("cannon"))
            {
                GameObject cannonTilemapObject = GameObject.Find("cannon");
                if (cannonTilemapObject == null)
                {
                    cannonTilemapObject = new GameObject("cannon");
                    cannonTilemapObject.layer = BoardView.Match3BoardLayerIndex;
                    var cannonTilemap = cannonTilemapObject.AddComponent<Tilemap>();
                    boardEditor.cannonTilemap = cannonTilemap;
                    var renderer = cannonTilemapObject.AddComponent<TilemapRenderer>();
                    renderer.sortingOrder = 5;
                    cannonTilemapObject.transform.SetParent(boardEditor.transform, false);
                }
                GridPaintingState.scenePaintTarget = cannonTilemapObject;
            }
            else if (currentPalette.Contains("ironwall"))
            {
                CreateOrSelectLayer("iron-wall", ref boardEditor.ironWallTilemap, 7);
            }
            else if (currentPalette.Contains("bucket"))
            {
                CreateOrSelectLayer("bucket", ref boardEditor.bucketTilemap, 5);
            }
            else if (currentPalette.Contains("booster"))
            {
                CreateOrSelectLayer("booster", ref boardEditor.boosterTilemap, 5);
            }
            else if (currentPalette.Contains("candle"))
            {
                CreateOrSelectLayer("candle", ref boardEditor.candleTilemap, 5);
            }
            else if (currentPalette.Contains("lockquest"))
            {
                CreateOrSelectLayer("lockquest", ref boardEditor.lockQuestTilemap, 9);
            }
            else if (currentPalette.Contains("lock"))
            {
                CreateOrSelectLayer("lock", ref boardEditor.lockTilemap, 8);
            }
            else if (currentPalette.Contains("parquet"))
            {
                CreateOrSelectLayer("parquet", ref boardEditor.lockTilemap, 1);
            }
            else if (currentPalette.Contains("meter"))
            {
                CreateOrSelectLayer("meter", ref boardEditor.meterTilemap, 10);
            }
   


        }

        private void CreateOrSelectLayer(string layerName, ref Tilemap tilemap, int sortOrder)
        {
            GameObject tilemapGameObject = GameObject.Find(layerName);
            if (tilemapGameObject == null)
            {
                tilemapGameObject = new GameObject(layerName) { layer = BoardView.Match3BoardLayerIndex };
                var newTilemap = tilemapGameObject.AddComponent<Tilemap>();
                tilemap = newTilemap;
                var renderer = tilemapGameObject.AddComponent<TilemapRenderer>();
                renderer.sortingOrder = sortOrder;
                tilemapGameObject.transform.SetParent(boardEditor.transform, false);
            }
            GridPaintingState.scenePaintTarget = tilemapGameObject;
        }
#endif
    }
}