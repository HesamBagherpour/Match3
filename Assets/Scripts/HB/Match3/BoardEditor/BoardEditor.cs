using System.Collections.Generic;
using Garage.Core.DI;
using Garage.Match3.BoardEditor;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using HB.Match3.BoardModules;
using HB.Match3.Cells;
using HB.Match3.Cells.Modules;
using HB.Match3.View;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;
using IModule = HB.Match3.Cells.IModule;

namespace HB.Match3.BoardEditor
{

    public class GameData
    {
        
    }
    // Each data in tilemap layer represents a data in board 
    // First we have cell tilemap. this tilemap shows active cells in board so we will create empty cell gameobjects on top of theses cells without any additional renderers
    // Then we have block tilemap layer. each existing sprites in this layer will be used for dedicated blocks at the begining of the game
    public class BoardEditor : MonoBehaviour
    {
        [SerializeField] Tilemap cellTilemap;
        [SerializeField] Tilemap blockTilemap;
        [SerializeField] Tilemap sofaTilemap;
        [SerializeField] Tilemap glassTilemap;
        [SerializeField] Tilemap woodTilemap;
        [SerializeField] Tilemap grassTilemap;
        [SerializeField] public Tilemap cannonTilemap;
        [SerializeField] public Tilemap ironWallTilemap;
        [SerializeField] public Tilemap bucketTilemap;
        [SerializeField] public Tilemap boosterTilemap;
        [SerializeField] public Tilemap candleTilemap;
        [SerializeField] public Tilemap lockTilemap;
        [SerializeField] public Tilemap lockQuestTilemap;
        [SerializeField] public PuzzleController puzzleController;
        [SerializeField] public Tilemap parquetTilemap;
        [SerializeField] public Tilemap meterTilemap;
        [SerializeField] BoardData boardData;
        [SerializeField] BoardViewData boardViewData;

        private void Awake()
        {
            //MessagePackHelper.ConfigMessagePack();

            if (boardData != null)
            {
                SaveLevel();
                GameData t = new GameData();
                //puzzleController.Enter(boardData, t);
                Destroy(gameObject);
            }
        }
        
        [Button]
        public void ResaveLevel()
        {
            Debug.Log("Resave Level");
            LoadLevel(true);
            SaveLevel();
        }

        [Button]
        public void ClearLevel()
        {
            Debug.Log("Clear Level");
            cellTilemap.ClearAllTiles();
            blockTilemap.ClearAllTiles();
            sofaTilemap.ClearAllTiles();
            glassTilemap.ClearAllTiles();
            woodTilemap.ClearAllTiles();
            grassTilemap.ClearAllTiles();
            cannonTilemap.ClearAllTiles();
            ironWallTilemap.ClearAllTiles();
            bucketTilemap.ClearAllTiles();
            boosterTilemap.ClearAllTiles();
            candleTilemap.ClearAllTiles();
            lockTilemap.ClearAllTiles();
            lockQuestTilemap.ClearAllTiles();
            parquetTilemap.ClearAllTiles();
            meterTilemap.ClearAllTiles();
        }

        [Button]
        public void LoadLevel_MessagePack()
        {
            LoadLevel(true);
        }
        [Button]
        public void LoadLevel_Json()
        {
            LoadLevel(false);
        }
        public void LoadLevel(bool withMessagePack)
        {
            ClearLevel();
            Debug.Log("BoardEditor: LoadLevel");
            boardData.Deserialize(withMessagePack);
            int width = boardData.width;
            int height = boardData.height;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int oneDArrayIndex = i + (j * width);
                    Cell cell = boardData.cells[oneDArrayIndex];
                    Vector3Int cellWorldPosition =
                        new Vector3Int(boardData.boardOffset.x + i, boardData.boardOffset.y + j, 0);

                    SetTileInTilemap<VisibleModule>(cellTilemap, cellWorldPosition, cell);
                    if (cell.IsBoosterHost)
                    {
                        CellTile tile = boardViewData.layerViewData.allTiles.Find(x => x.name == "background-tile-booster-host");
                        cellTilemap.SetTile(cellWorldPosition, tile);
                    }
                    SetTileInTilemap<SpawnerModule>(cellTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<ExitModule>(cellTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<InvisibleBlocker>(cellTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<BlockModule>(blockTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<GlassModule>(glassTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<WoodIronModule>(woodTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<GrassModule>(grassTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<CannonModule>(cannonTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<IronWallModule>(ironWallTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<BucketModule>(bucketTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<BoosterModule>(boosterTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<CandleModule>(candleTilemap, cellWorldPosition, cell);
                    SetLockTileInTilemap(lockTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<LockQuestModule>(lockQuestTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<ParquetModule>(parquetTilemap, cellWorldPosition, cell);
                    SetTileInTilemap<MeterSpawnerModule>(meterTilemap, cellWorldPosition, cell);


                }
            }

            for (int i = 0; i < boardData.sofaList.Count; i++)
            {
                SofaModule sofaModule = boardData.sofaList[i];
                Vector3Int sofaWorldPosition =
                    new Vector3Int(boardData.boardOffset.x + sofaModule.position.x,
                        boardData.boardOffset.y + sofaModule.position.y,
                        0);

                CellTile cellTile = boardViewData.layerViewData.allTiles.Find(x => x.name == sofaModule.id);
                SofaTile sofaTile = (SofaTile)cellTile;
                // TileData tileData = new TileData();
                // sofaTile.GetTileData(sofaWorldPosition, sofaTilemap as ITilemap, ref tileData);
                sofaTilemap.SetTile(sofaWorldPosition, sofaTile);
                sofaTilemap.SetTransformMatrix(sofaWorldPosition, Matrix4x4.TRS(sofaTile.offsetPosition, Quaternion.identity, sofaTile.scale));
                sofaTilemap.RefreshTile(sofaWorldPosition);
            }


#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(boardData);

#endif

        }

        private void SetTileInTilemap<T>(Tilemap tilemap, Vector3Int pos, Cell cell) where T : class, IModule
        {
            T module = cell.GetModule<T>();
            if (module is BaseModule baseModule)
            {
                CellTile tile = boardViewData.layerViewData.allTiles.Find(x => x.name == baseModule.id);
                if (baseModule.id == null)
                {
                    Debug.LogError($"{module.GetType()} is missing id");
                }
                if (tile == null) Debug.LogError($"Could not find tile with id: {baseModule.id}");
                tilemap.SetTile(pos, tile);
            }
        }
        private void SetLockTileInTilemap(Tilemap tilemap, Vector3Int pos, Cell cell)
        {
            LockModule lockModule = cell.GetModule<LockModule>();
            if (lockModule != null)
            {
                var tileName = lockModule.layerName + "-" + lockModule.color.ToString().ToLower();
                CellTile tile = boardViewData.layerViewData.allTiles.Find(x => x.name == tileName);
                if (lockModule.id == null)
                {
                    Debug.LogError($"{lockModule.GetType()} is missing id");
                }
                if (tile == null) Debug.LogError($"Could not find tile with tileName: {tileName}");
                tilemap.SetTile(pos, tile);
            }
        }

        [Button]
        public void SaveLevel()
        {
            Assert.IsTrue(cellTilemap != null && blockTilemap != null);
            BoundsInt cellBounds = cellTilemap.cellBounds;
            // Debug.Log("x min and max: " + cellBounds.xMin + "," + cellBounds.xMax);
            int zMin = cellBounds.zMin;

            //Cell[,] cells = new Cell[cellBounds.xMax - cellBounds.xMin, cellBounds.yMax - cellBounds.yMin];

            int width = cellBounds.xMax - cellBounds.xMin;
            int height = cellBounds.yMax - cellBounds.yMin;

            boardData.cells = new Cell[width * height];
            boardData.sofaList = new List<SofaModule>();
            // Generate cell gameObjects for each tilemap cell and fill the array
            Vector2Int boardOffset = new Vector2Int(cellBounds.xMin, cellBounds.yMin);
            for (int x = cellBounds.xMin; x < cellBounds.xMax; x++)
            {
                for (int y = cellBounds.yMin; y < cellBounds.yMax; y++)
                {
                    int xPos = x - cellBounds.xMin;
                    int yPos = y - cellBounds.yMin;
                    Vector3Int cellPositionOnTilemap = new Vector3Int(x, y, zMin);
                    // Generate cell objects
                    if (cellTilemap.HasTile(cellPositionOnTilemap))
                    {
                        // Create a new cell
                        Cell cell = new Cell();
                        if (cellTilemap.GetSprite(cellPositionOnTilemap).name == "spawner")
                        {
                            var spawnerModule = cell.AddModule<SpawnerModule>();
                            spawnerModule.Setup("spawner");
                        }
                        else if (cellTilemap.GetSprite(cellPositionOnTilemap).name == "cross")
                        {
                            CellTile restrictionTile = (CellTile)cellTilemap.GetTile(cellPositionOnTilemap);
                            var invisibleBlocker = cell.AddModule<InvisibleBlocker>();
                            invisibleBlocker.Restriction = restrictionTile.restriction;
                            invisibleBlocker.Setup("invisible-blocker");
                        }
                        else if (cellTilemap.GetSprite(cellPositionOnTilemap).name == "Arrow")
                        {
                            var exitModule = cell.AddModule<ExitModule>();
                            exitModule.Setup(ExitModule.LayerName);
                        }
                        else
                        {
                            CellTile ct = (CellTile)cellTilemap.GetTile(cellPositionOnTilemap);
                            var visibleModule = cell.AddModule<VisibleModule>();
                            visibleModule.Setup("background-tile");
                            if (ct.name.Contains("booster")) cell.IsBoosterHost = true;
                        }

                        // Add board position component
                        cell.position = new Point(xPos, yPos);
                        AddBlockModule(cellPositionOnTilemap, cell);
                        AddCannonModule(cellPositionOnTilemap, cell);
                        AddBucketModule(cellPositionOnTilemap, cell);

                        AddBoosterModule(cellPositionOnTilemap, cell);
                        AddCandleModule(cellPositionOnTilemap, cell);
                        AddMeterSpawner(cellPositionOnTilemap, cell);

                        AddIronWallModule<IronWallModule>(ironWallTilemap, cell, cellPositionOnTilemap);
                        AddRestrictionModule<WoodIronModule>(woodTilemap, cell, cellPositionOnTilemap);
                        AddRestrictionModule<GlassModule>(glassTilemap, cell, cellPositionOnTilemap);
                        AddRestrictionModule<GrassModule>(grassTilemap, cell, cellPositionOnTilemap);


                        AddLockModule(lockTilemap, cell, cellPositionOnTilemap);
                        AddRestrictionModule<LockQuestModule>(lockQuestTilemap, cell, cellPositionOnTilemap);

                        AddParquetModule(cellPositionOnTilemap, cell);

                        boardData.cells[xPos + (yPos * width)] = cell;
                    }
                    else
                    {
                        Cell cell = new Cell
                        {
                            position = new Point() { x = xPos, y = yPos }
                        };

                        // Add board position component
                        boardData.cells[xPos + (yPos * width)] = cell;
                    }
                }
            }

            for (int x = cellBounds.xMin; x < cellBounds.xMax; x++)
            {
                for (int y = cellBounds.yMin; y < cellBounds.yMax; y++)
                {
                    int xPos = x - cellBounds.xMin;
                    int yPos = y - cellBounds.yMin;
                    Vector3Int cellPositionOnTilemap = new Vector3Int(x, y, zMin);
                    if (sofaTilemap.HasTile(cellPositionOnTilemap))
                    {
                        SofaTile sofaTile = sofaTilemap.GetTile<SofaTile>(cellPositionOnTilemap);
                        if (sofaTile != null)
                        {
                            SofaModule sofaModule = new SofaModule
                            {
                                position = new Point(xPos, yPos),
                                size = new Point(sofaTile.size.x, sofaTile.size.y),
                                id = sofaTile.name,
                                layerName = sofaTilemap.name,
                                order = sofaTilemap.GetComponent<TilemapRenderer>().sortingOrder
                            };
                            boardData.sofaList.Add(sofaModule);
                        }
                    }
                }
            }


            boardData.width = width;
            boardData.height = height;
            boardData.boardOffset = boardOffset;
            //boardData.blockTypes = blockViewDatabase.GetBlockTypes();

            boardData.Serialize();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(boardData);
            Debug.Log("Board Data saved");
#endif

        }

        


        private void AddBlockModule(Vector3Int cellPositionOnTilemap, Cell cell)
        {
            // Setup initial block type for cells 
            if (blockTilemap.HasTile(cellPositionOnTilemap))
            {
                BlockModule blockModule = cell.AddModule<BlockModule>();
                TileBase tile = blockTilemap.GetTile(cellPositionOnTilemap);
                BlockTile blockTile = tile as BlockTile;
                blockModule.Setup(blockTile.name);
                blockModule.blockType = blockTile.blockViewData.blockType;
                blockModule.restrictionType = blockTile.blockViewData.restrictionType;
            }
        }

        private void AddMeterSpawner(Vector3Int cellPositionOnTilemap, Cell cell)
        {
            // Setup initial block type for cells 
            if (meterTilemap.HasTile(cellPositionOnTilemap))
            {
                MeterSpawnerModule meterSpawnerModule = cell.AddModule<MeterSpawnerModule>();
                TileBase tile = meterTilemap.GetTile(cellPositionOnTilemap);
                CellTile blockTile = tile as CellTile;
                meterSpawnerModule.Setup(blockTile.name);
            }
        }


        private void AddBoosterModule(Vector3Int cellPositionOnTilemap, Cell cell)
        {
            if (boosterTilemap.HasTile(cellPositionOnTilemap))
            {
                BoosterModule boosterModule = cell.AddModule<BoosterModule>();
                BoosterTile boosterTile = boosterTilemap.GetTile(cellPositionOnTilemap) as BoosterTile;
                Debug.Log("Add booster with type " + boosterTile.boosterType + " to cell at " + cell.position);
                boosterModule.Setup(boosterTile.boosterType.ToString());
                boosterModule.SetData(boosterTile.boosterType, cell.position, false);
            }
        }
        private void AddCannonModule(Vector3Int cellPositionOnTilemap, Cell cell)
        {
            if (cannonTilemap != null && cannonTilemap.HasTile(cellPositionOnTilemap))
            {
                string cannonSpriteName = cannonTilemap.GetSprite(cellPositionOnTilemap).name;
                var cannonTile = (GameObjectTile)cannonTilemap.GetTile(cellPositionOnTilemap);
                CannonModule cannonModule = cell.AddModule<CannonModule>();
                cannonModule.Setup(cannonTile.name);
                TilemapRenderer tilemapRenderer = cannonTilemap.transform.GetComponent<TilemapRenderer>();
                cannonModule.order = tilemapRenderer.sortingOrder;
                if (cannonSpriteName.Contains("up"))
                {
                    cannonModule.direction = Direction.Top;
                }
                else if (cannonSpriteName.Contains("down"))
                {
                    cannonModule.direction = Direction.Bottom;
                }
                else if (cannonSpriteName.Contains("left"))
                {
                    cannonModule.direction = Direction.Left;
                }
                else if (cannonSpriteName.Contains("right"))
                {
                    cannonModule.direction = Direction.Right;
                }
            }
        }
        private void AddBucketModule(Vector3Int cellPositionOnTilemap, Cell cell)
        {
            if (bucketTilemap != null && bucketTilemap.HasTile(cellPositionOnTilemap))
            {
                var bucketTile = (BucketTile)bucketTilemap.GetTile(cellPositionOnTilemap);
                BucketModule module = cell.AddModule<BucketModule>();
                module.Setup(bucketTile.name);
                module.Restriction = bucketTile.restriction;
                module.color = bucketTile.color;
                module.layerName = "BucketView";
                TilemapRenderer tilemapRenderer = bucketTilemap.transform.GetComponent<TilemapRenderer>();
                module.order = tilemapRenderer.sortingOrder;
            }
        }

        //AddFlowerModule
        private void AddCandleModule(Vector3Int cellPositionOnTilemap, Cell cell)
        {
            if (candleTilemap != null && candleTilemap.HasTile(cellPositionOnTilemap))
            {
                var candleTile = (CandleTile)candleTilemap.GetTile(cellPositionOnTilemap);
                CandleModule module = cell.AddModule<CandleModule>();
                module.Setup(candleTile.name);
                module.active = candleTile.candleIsOn;
                module.Restriction = candleTile.restriction;
                module.layerName = "CandleView";
                TilemapRenderer tilemapRenderer = candleTilemap.transform.GetComponent<TilemapRenderer>();
                module.order = tilemapRenderer.sortingOrder;
            }
        }

        private void AddRestrictionModule<T>(Tilemap tileMap, Cell cell, Vector3Int cellPositionOnTilemap)
            where T : IModule, new()
        {
            if (tileMap != null && tileMap.HasTile(cellPositionOnTilemap))
            {
                CellTile restrictionTile = (CellTile)tileMap.GetTile(cellPositionOnTilemap);
                T _module = cell.AddModule<T>();
                var module = _module as RestrictionModule;
                module.Restriction = restrictionTile.restriction;
                module.Setup(restrictionTile.name);
                module.layerName = tileMap.name;
                TilemapRenderer tilemapRenderer = tileMap.transform.GetComponent<TilemapRenderer>();
                module.order = tilemapRenderer.sortingOrder;
            }
        }
        private void AddLockModule(Tilemap tileMap, Cell cell, Vector3Int cellPositionOnTilemap)
        {
            if (tileMap != null && tileMap.HasTile(cellPositionOnTilemap))
            {
                LockTile lockTile = (LockTile)tileMap.GetTile(cellPositionOnTilemap);
                LockModule _module = cell.AddModule<LockModule>();
                _module.Restriction = lockTile.restriction;
                _module.Setup("lock-tile");
                _module.layerName = "lock-tile";
                _module.color = lockTile.blockColor;
                TilemapRenderer tilemapRenderer = tileMap.transform.GetComponent<TilemapRenderer>();
                _module.order = tilemapRenderer.sortingOrder;
            }
        }
        private void AddIronWallModule<T>(Tilemap tileMap, Cell cell, Vector3Int cellPositionOnTilemap)
            where T : IModule, new()
        {
            if (tileMap != null && tileMap.HasTile(cellPositionOnTilemap))
            {
                CellTile cellTile = (CellTile)tileMap.GetTile(cellPositionOnTilemap);
                T _module = cell.AddModule<T>();
                var module = _module as IronWallModule;
                module.Restriction = cellTile.restriction;
                module.Setup(cellTile.name);
                module.layerName = tileMap.name;
                TilemapRenderer tilemapRenderer = tileMap.transform.GetComponent<TilemapRenderer>();
                module.order = tilemapRenderer.sortingOrder;
            }
        }
        private void AddParquetModule(Vector3Int cellPositionOnTilemap, Cell cell)
        {
            if (parquetTilemap.HasTile(cellPositionOnTilemap))
            {
                ParquetModule parquetModule = cell.AddModule<ParquetModule>();
                TileBase tile = parquetTilemap.GetTile(cellPositionOnTilemap);
                CellTile parquetTile = tile as CellTile;
                parquetModule.Setup(parquetTile.name);
                parquetModule.layerName = parquetTilemap.name;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (lockQuestTilemap == null) return;
            BoundsInt cellBounds = cellTilemap.cellBounds;
            int zMin = cellBounds.zMin;
            int width = cellBounds.xMax - cellBounds.xMin;
            int height = cellBounds.yMax - cellBounds.yMin;
            // Generate cell gameObjects for each tilemap cell and fill the array
            Vector2Int boardOffset = new Vector2Int(cellBounds.xMin, cellBounds.yMin);
            for (int x = cellBounds.xMin; x < cellBounds.xMax; x++)
            {
                for (int y = cellBounds.yMin; y < cellBounds.yMax; y++)
                {
                    int xPos = x - cellBounds.xMin;
                    int yPos = y - cellBounds.yMin;
                    Vector3Int cellPositionOnTilemap = new Vector3Int(x, y, zMin);
                    // Generate cell objects
                    if (lockQuestTilemap.HasTile(cellPositionOnTilemap))
                    {
                        UnityEditor.Handles.Label(lockQuestTilemap.GetCellCenterWorld(cellPositionOnTilemap), $"({xPos},{yPos})");
                    }
                }
            }

        }
#endif
    }
}