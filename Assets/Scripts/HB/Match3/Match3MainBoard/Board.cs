using System;
using System.Collections.Generic;
using HB.Match3.Behaviours;
using HB.Match3.Block;
using HB.Match3.Board;
using HB.Match3.Board.BoardStates;
using HB.Match3.Cell;
using HB.Match3.Models;
using HB.Match3.Modules;
using HB.Match3.View;
using HB.Match3.View.Quest;
using HB.Packages.Logger;
using HB.Packages.StateMachine;
using HB.Packages.Timers;
using UnityEngine;
using PossibleMatch = HB.Match3.Board.PossibleMatch;
using Random = System.Random;

namespace HB.Match3.Match3MainBoard
{
    public class Board
    {
        #region Public Fields

        public SwapData LastValidSwap;
        public event Action OnSuccessMove;
        public event Action OnReshuffle;
        public event Action OnFingerBoosterUsed;
        public event Action<int> OnAddMoves;

        public int NumberOfTimesMovePurchased;
        public int NumberOfInGameBoosterUsed;
        public int NumberOFPreGameBoosterUsed;

        #endregion

        #region Private Fields

        private readonly SwapBehaviour _swapBehaviour;
        private readonly MatchPredictor _matchPredictor;
        private readonly MatchBehaviour _matchBehaviour;

        private readonly FallBehaviour _fallBehaviour;

        public BlockType GetNestedBlockType(BlockType blockType)
        {
            return nestedBlockTypes.ContainsKey(blockType) ? nestedBlockTypes[blockType] : BlockType.None;
        }

        private readonly CreateBehaviour _createBehaviour;
        private readonly IBoardView _boardView;
        private readonly int _randomSeed;
        private readonly Random _random;
        private readonly BlockFactory _blockFactory;
        public List<SofaModule> SofaList { get; private set; }
        private Fsm<Board> _fsm;
        public static bool IsActive { get; private set; }
        private List<BoosterType> _initialBoosters;
        private Dictionary<BlockType, BlockType> nestedBlockTypes;
        private BoosterType fingerBooster;

        #endregion

        #region Public Properties

        public MyCell[,] Cells { get; }

        public int Width { get; }

        public int Height { get; }

        public List<HB.Match3.Match.MatchInfo> MatchInfos { get; private set; }
        public PossibleMatch PossibleMatch { get; set; }
        public List<BoosterInfo> BoosterInfo { get; set; }
        public static Point Offset { get; private set; }
        public BoardFlow BoardFlow { get; private set; }
        internal BlockType RandomBlockType => _blockFactory.GetRandomBlockType(false);

        #endregion

        #region  Constructors

        public Board(BoardData data, IBoardView boardView)
        {
            Log.Debug("Board", $"Create a new board");
            IsActive = true;
            Width = data.width;
            Height = data.height;
            _boardView = boardView;
            MatchInfos = new List<HB.Match3.Match.MatchInfo>();
            nestedBlockTypes = new Dictionary<BlockType, BlockType>();
            Cells = new MyCell[Width, Height];
            LastValidSwap = new SwapData();
            InitialCells(data);
            SofaList = data.sofaList;

            Offset = new Point(data.boardOffset.x, data.boardOffset.y);

            _matchPredictor = new MatchPredictor(this);
            _swapBehaviour = new SwapBehaviour(this, _matchPredictor);
            _matchBehaviour = new MatchBehaviour(this);
            BlockType[] allBlockTypes = new BlockType[data.blockTypes.Length];
            for (int i = 0; i < data.blockTypes.Length; i++)
            {
                BlockType blockType = data.blockTypes[i].blockType;
                allBlockTypes[i] = blockType;
                if (nestedBlockTypes.ContainsKey(blockType) == false)
                {
                    BlockViewData nestedBlockType = data.blockTypes[i].nestedBlockType;
                    if (nestedBlockType != null) nestedBlockTypes[blockType] = nestedBlockType.blockType;
                    else nestedBlockTypes[blockType] = BlockType.None;
                }
            }

            if (_randomSeed == 0) _randomSeed = GameTime.Now().Millisecond;
            _random = new Random(_randomSeed);
            _blockFactory = new BlockFactory(allBlockTypes, _boardView.GetBlockViewData(), _random);

            _createBehaviour = new CreateBehaviour(this, boardView, _blockFactory, data, _random);
            _fallBehaviour = new FallBehaviour(this, _blockFactory);
            SetupFsm();
            if (BoardFlow == null) CreateNewBoardFlow();
        }

        internal bool HasFingerBooster()
        {
            return fingerBooster != BoosterType.None;
        }

        public void CreateNewBoardFlow()
        {
            BoardFlow = new BoardFlow(this);
        }

        public void AddInitialBoosters()
        {
            if (_initialBoosters == null) return;
            // foreach (var cell in Cells)
            // {
            //     if (cell.Contains<BoosterModule>())
            //     {
            //         _initialBoosters.Clear();
            //         return;
            //     }
            // }
            for (int i = 0; i < _initialBoosters.Count; i++)
            {
                MyCell cell = GetCellForInitialBooster(_initialBoosters[i]);
                if (cell != null) GetOrAddBoosterModule(cell, _initialBoosters[i]);
            }
        }

        private MyCell GetCellForInitialBooster(BoosterType boosterType)
        {
            if (boosterType == BoosterType.JumboBlock)
            {
                // Return Booster host cell if it contains a quest block
                foreach (var _cell in Cells)
                {
                    var blockModule = _cell.GetModule<BlockModule>();
                    if (blockModule != null &&
                        blockModule.IsJumbo == false &&
                        _cell.IsBoosterHost &&
                        QuestGiver.IsInQuest(blockModule) &&
                        _cell.Contains<BoosterModule>() == false &&
                        _cell.IsLocked(ActionType.Swap, Direction.Center) == false) return _cell;
                }

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        MyCell cell = Cells[x, y];
                        var blockModule = cell.GetModule<BlockModule>();
                        if (cell.IsVisible == false ||
                            cell.IsLocked(ActionType.Swap, Direction.All) ||
                            blockModule == null ||
                            blockModule.id == BlockIDs.Plant ||
                            blockModule.IsJumbo == true ||
                            QuestGiver.IsInQuest(blockModule) == false ||
                            cell.Contains<BoosterModule>())
                        {
                            continue;
                        }
                        else return cell;
                    }
                }

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        MyCell cell = Cells[x, y];
                        var blockModule = cell.GetModule<BlockModule>();
                        if (cell.IsVisible == false ||
                            cell.IsLocked(ActionType.Swap, Direction.All) ||
                            blockModule == null ||
                            blockModule.IsJumbo == true ||
                            cell.Contains<BoosterModule>())
                        {
                            continue;
                        }
                        else return cell;
                    }
                }
                return null;
            }
            else
            {
                foreach (var _cell in Cells)
                {
                    if (_cell.IsBoosterHost &&
                        _cell.Contains<BlockModule>() == true &&
                        _cell.Contains<BoosterModule>() == false &&
                        _cell.IsLocked(ActionType.Swap, Direction.Center) == false) return _cell;
                }
                int rx = _random.Next(0, Width);
                int ry = _random.Next(0, Height);
                MyCell cell = Cells[rx, ry];
                while (cell.IsVisible == false ||
                       cell.IsLocked(ActionType.Swap, Direction.All) ||
                       cell.Contains<BlockModule>() == false ||
                       cell.Contains<BoosterModule>())
                {
                    rx = _random.Next(0, Width);
                    ry = _random.Next(0, Height);
                    cell = Cells[rx, ry];
                }
                return cell;
            }
        }

        public BoosterModule GetOrAddBoosterModule(MyCell cell, BoosterType type)
        {
            if (type == BoosterType.JumboBlock)
            {
                var blockModule = cell.GetModule<BlockModule>();
                if (blockModule != null && blockModule.blockType.id == BlockIDs.Simple)
                {
                    blockModule.SetJumbo();
                    if (QuestGiver.IsInQuest(blockModule)) blockModule.AddCount(3);
                }
                return null;
            }
            else
            {
                BoosterModule boosterModule = cell.GetModule<BoosterModule>();
                if (boosterModule == null)
                {
                    boosterModule = cell.AddModule<BoosterModule>();
                    cell.View.AddModuleView(cell.position, boosterModule);
                    boosterModule.SetBoard(this);
                }
                boosterModule.SetData(type, cell.position);
                return boosterModule;
            }
        }

        public void CreateSofaViews()
        {
            if (SofaList != null)
            {
                for (int i = 0; i < SofaList.Count; i++)
                {
                    SofaModule sofaModule = SofaList[i];
                    _boardView.CreateView(sofaModule, sofaModule.position);
                    sofaModule.Init(this);
                }
            }
        }

        private void InitialCells(BoardData data)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Cells[x, y] = data.cells[x + (y * Width)];
                }
            }
        }

        public void InvokeReshuffle()
        {
            OnReshuffle?.Invoke();
            _boardView.Reshuffle();
        }

        public void Dispose()
        {
            IsActive = false;
            ClearBoosterUnderFinger();
            _fsm = null;
            MatchInfos?.Clear();
            _initialBoosters?.Clear();
            MatchInfos = null;
            LastValidSwap = null;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Cells[x, y].Dispose();
                }
            }

            for (int i = 0; i < SofaList.Count; i++)
            {
                SofaList[i].Dispose();
            }

            _boardView.Dispose();
        }

        #endregion

        #region Unity

        public void ReduceAMove() => OnSuccessMove?.Invoke();
        public void AddMoves(int value) => OnAddMoves?.Invoke(value);

        public void Update(float deltaTime)
        {
            if (IsActive)
            {
                _fsm.Update(deltaTime);
#if UNITY_EDITOR
                BoardFlow.Visualize();
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
                {
                    CacheInitialBoosters(new List<BoosterType>() { BoosterType.Cross });
                    AddInitialBoosters();
                }
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
                {
                    CacheInitialBoosters(new List<BoosterType>() { BoosterType.Rainbow });
                    AddInitialBoosters();
                }
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3))
                {
                    CacheInitialBoosters(new List<BoosterType>() { BoosterType.JumboBlock });
                    AddInitialBoosters();
                }
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4))
                {
                    CacheInitialBoosters(new List<BoosterType>() { BoosterType.Horizontal });
                    AddInitialBoosters();
                }
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha5))
                {
                    CacheInitialBoosters(new List<BoosterType>() { BoosterType.Vertical });
                    AddInitialBoosters();
                }

                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.O))
                {
                    const BoosterType pw = BoosterType.Rainbow;
                    Debug.Log($"Set {pw} Powerup under finger");
                    CacheBoosterUnderFinger(pw);
                }
#endif
            }
        }

        public void ExecuteFingerBooster(MyCell cell, Action OnClear)
        {
            if (cell.Contains<BucketModule>() || cell.Contains<CannonModule>() || cell.IsVisible == false) return;
            if (fingerBooster == BoosterType.Hammer)
            {
                cell.AddMatchType(HB.Match3.Models.MatchType.Booster);
                cell.Hit(HitType.Direct, 1);
                MatchState.IgnoreAdjacent = true;
                MatchInfos.Add(new HB.Match3.Match.MatchInfo()
                {
                    matchType = HB.Match3.Models.MatchType.Booster,
                    OriginCell = cell,
                    MatchedCells = new List<MyCell>() { cell }
                });

                _boardView.PlayEffect(cell.position, "hammer", () => { OnClear?.Invoke(); });
                BoardView.PlayAudio("Hammer");
                ClearBoosterUnderFinger();
                OnFingerBoosterUsed?.Invoke();
            }
            else if (fingerBooster != BoosterType.None)
            {
                if (fingerBooster == BoosterType.Rainbow)
                {
                    BlockModule block = cell.GetModule<BlockModule>();
                    if (cell.Contains<CandleModule>() || cell.Contains<WoodIronModule>() || (block != null && block.blockType.id == BlockIDs.Plant)) return;
                    SwapState.PassedWithRainbowPowerUp = true;
                }
                GetOrAddBoosterModule(cell, fingerBooster);
                cell.AddMatchType(HB.Match3.Models.MatchType.Booster);
                cell.Hit(HitType.Direct, 1);
                cell.ClearBooster(HitType.Direct, 1, null);
                MatchInfos.Add(new HB.Match3.Match.MatchInfo()
                {
                    matchType = HB.Match3.Models.MatchType.Booster,
                    OriginCell = cell,
                    MatchedCells = new List<MyCell>() { cell }
                });
                ClearBoosterUnderFinger();
                OnFingerBoosterUsed?.Invoke();
                OnClear?.Invoke();
            }
        }

        public void CacheBoosterUnderFinger(BoosterType booster)
        {
            fingerBooster = booster;
            _boardView.PlayFingerBoosterEffect();
        }

        public void ClearBoosterUnderFinger()
        {
            fingerBooster = BoosterType.None;
            _boardView.ReleaseFingerBoosterEffect();
        }

        #endregion

        #region Public Methods

        public bool HasEmptyCell(Point pos)
        {
            return HasEmptyCell(pos.x, pos.y);
        }

        public bool HasEmptyCell(int x, int y)
        {
            if (x > Width - 1 || x < 0) return false;
            if (y > Height - 1 || y < 0) return false;
            return Cells[x, y].IsVisible;
        }

        public bool HasSpawnerCell(Point pos)
        {
            if (pos.x > Width - 1 || pos.x < 0) return false;
            if (pos.y > Height - 1 || pos.y < 0) return false;
            return Cells[pos.x, pos.y].Contains<SpawnerModule>();
        }

        public bool IsInBounds(Point pos)
        {
            return IsInBounds(pos.x, pos.y);
        }

        public bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < Width &&
                   y >= 0 && y < Height;
        }

        public bool IsAdjacent(Point pos, Point otherPos)
        {
            return (pos.x == otherPos.x || pos.y == otherPos.y) &&
                   (pos.x == otherPos.x + 1 || pos.x == otherPos.x - 1 ||
                    pos.y == otherPos.y + 1 || pos.y == otherPos.y - 1);
        }

        public bool IsIgnoredBlockType(BlockType blockType)
        {
            return blockType == BlockType.None;
        }

        #endregion

        #region Private Methods

        private void SetupFsm()
        {
            // IfMatchFound ifFoundMatch = new IfMatchFound(this, _matchPredictor);
            // IfBoosterHit ifBoosterHit = new IfBoosterHit();
            // IfPostClearNeedNewClear ifPostClearNeedNewClear = new IfPostClearNeedNewClear();
            //
            // InitState init = new InitState { Name = "Board:Init" };
            // CreateState create = new CreateState(_createBehaviour, _boardView) { Name = "Board:Create" };
            // CannonFireState cannonState = new CannonFireState() { Name = "Board:Cannon" };
            // SwapState swap = new SwapState(_swapBehaviour, _boardView) { Name = "Board:Swap" };
            // MatchState match = new MatchState(_matchBehaviour, _boardView) { Name = "Board:Match" };
            //
            // DetectBoosterState detectBooster = new DetectBoosterState(_boardView) { Name = "Board:Detect Booster" };
            // HitBoosterState hitBooster = new HitBoosterState() { Name = "Board:Hit Boosters" };
            // AddBoosterState addBooster = new AddBoosterState(_boardView, _blockFactory) { Name = "Board:Add New Boosters" };
            //
            // DetectJumboState detectJumbo = new DetectJumboState() { Name = "Board:Detect Jumbo" };
            // // HitJumboState hitJumbo = new HitJumboState() { Name = "Board: Hit Jumbo" };
            // AddJumboState addJumbo = new AddJumboState(_blockFactory, detectJumbo) { Name = "Board:Add Jumbo" };
            // // ClearJumboState /
            // MergeCells merge = new MergeCells() { Name = "Board:Merge" };
            // ClearState clear = new ClearState(_boardView, _blockFactory) { Name = "Board:Clear" };
            // PostClearState postClear = new PostClearState(_boardView, _blockFactory) { Name = "Board:Post Clear" };
            // // ActiveJumboState activateJumbo = new ActiveJumboState(_blockFactory, _boardView) { Name = "Board:Activate Jumbo" };
            // ClearSofas clearSofas = new ClearSofas() { Name = "Board:ClearSofa" };
            // CandleState candleState = new CandleState() { Name = "Board:Candle State" };
            // LockState lockState = new LockState() { Name = "Board:Lock State" };
            // ActiveBucketsState activeBuckets = new ActiveBucketsState(_boardView, _blockFactory, _random) { Name = "Board:Active Buckets" };
            // ClearIronWalls clearIronWalls = new ClearIronWalls() { Name = "Board:ClearIronWalls" };
            // ClearFlagsState clearFlags = new ClearFlagsState() { Name = "Board:Clear Flags" };
            // ReshuffleState reshuffle = new ReshuffleState(_matchPredictor, _random) { Name = "Board:Reshuffle" };
            // FallState fall = new FallState(_fallBehaviour) { Name = "Board:Fall" };
            // MeterState meter = new MeterState(_blockFactory, _random, _boardView) { Name = "Board:Meter" };
            // FlowerState flower = new FlowerState(_blockFactory, _random, _boardView) { Name = "Board:Flower" };
            //
            //
            //
            // // JumboExplodedCondition jumboExplodedCondition = new JumboExplodedCondition(this);
            //
            // Transition.CreateAndAssign(init, create);
            // Transition.CreateAndAssign(create, match, cannonState, ifFoundMatch);
            // Transition.CreateAndAssign(cannonState, swap);
            // Transition.CreateAndAssign(swap, match);
            //
            // Transition.CreateAndAssign(match, detectBooster);
            //
            // Transition.CreateAndAssign(detectBooster, detectJumbo);
            // Transition.CreateAndAssign(detectJumbo, merge);
            //
            // Transition.CreateAndAssign(merge, flower);
            // Transition.CreateAndAssign(flower, clear);
            //
            // //Transition.CreateAndAssign(merge, clear);
            //
            // Transition.CreateAndAssign(clear, lockState);
            // Transition.CreateAndAssign(lockState, postClear);
            // Transition.CreateAndAssign(postClear, flower, hitBooster, ifPostClearNeedNewClear);
            // Transition.CreateAndAssign(hitBooster, flower, addBooster, ifBoosterHit);
            // Transition.CreateAndAssign(addBooster, addJumbo);
            //
            //
            // Transition.CreateAndAssign(addJumbo, clearSofas);
            // Transition.CreateAndAssign(clearSofas, activeBuckets);
            // Transition.CreateAndAssign(activeBuckets, clearIronWalls);
            // Transition.CreateAndAssign(clearIronWalls, candleState);
            // Transition.CreateAndAssign(candleState, clearFlags);
            // Transition.CreateAndAssign(clearFlags, fall);
            //
            // Transition.CreateAndAssign(fall, match, meter, ifFoundMatch);
            // Transition.CreateAndAssign(meter, reshuffle);
            // Transition.CreateAndAssign(reshuffle, match, cannonState, ifFoundMatch);

            //_fsm = new Fsm<Board>(this, init, true);
        }

        public void CacheInitialBoosters(List<BoosterType> boosterTypes)
        {
            _initialBoosters = boosterTypes;
        }

        public void LogAllCells(string prefix)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(prefix + "\n");
            foreach (var cell in Cells)
            {
                if (cell.IsVisible)
                {
                    sb.Append($"{cell.position} : ");
                    foreach (var module in cell.context.GetAll<BaseModule>())
                    {
                        sb.Append(module.id + " - ");
                    }
                    sb.Append("\n");
                }
            }
            Log.Debug("LogAllCells", $"Cells: {sb}");
        }

        #endregion
    }
}