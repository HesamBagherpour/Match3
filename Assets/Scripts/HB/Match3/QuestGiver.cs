using System;
using System.Collections.Generic;
using Garage.Match3;
using Garage.Match3.BoardStates;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using Garage.Match3.View.Quest;
using HB.Logger;
using HB.Match3.Cells.Modules;
using HB.Match3.View;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HB.Match3
{
    public class QuestGiver
    {
        public static int CurrentMoves { get; private set; }
        public static int Coupons { get; private set; }
        public static Action<Match3Result> OnFinished;
        private static Board _board;
        private static BoardView _boardView;
        private static Rect _movesPosition;
        private static List<QuestData> QuestData;
        private static List<string> FinishedQuestsInView;
        private static List<BlockQuestView> _questBlockTypes;
        public static List<Cell> LockQuests { get; private set; }
        public static bool IsFinished { get; private set; }

        public static BlockType RequiredBlockType()
        {
            for (int i = 0; i < _questBlockTypes.Count; i++)
            {
                var blockType = _questBlockTypes[i].blockViewData.blockType;
                if (blockType.id.Equals(BlockIDs.Meter)) continue;
                if (blockType.id.Equals(BlockIDs.flower)) continue;
                if (blockType.id.Equals(BlockIDs.Plant))
                {
                    var plantQuest = QuestData.FindIndex(x => x.QuestName == BlockIDs.Plant);
                    if (QuestData[plantQuest].count <= GetBlockTypeCountInBoard(blockType)) continue;
                }
                if (BoardHasBlockType(blockType, _questBlockTypes[i].MaxCount) == false)
                {
                    return blockType;
                }
            }
            return BlockType.None;
        }

        internal static void RemoveLockQuest(Cell cell)
        {
            if (LockQuests.Contains(cell)) LockQuests.Remove(cell);
        }

        private static bool BoardHasBlockType(BlockType blockType, int maxCount)
        {
            int counter = 0;
            if (counter == maxCount) return true;
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    var cell = _board.Cells[x, y];
                    if (cell.IsVisible)
                    {
                        if (cell.GetBlockType() == blockType)
                        {
                            counter++;
                            if (counter == maxCount) return true;
                        }
                    }
                }
            }
            return false;
        }

        private static int GetBlockTypeCountInBoard(BlockType blockType)
        {
            int counter = 0;
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    var cell = _board.Cells[x, y];
                    if (cell.IsVisible)
                    {
                        if (cell.GetBlockType() == blockType)
                        {
                            counter++;
                        }
                    }
                }
            }
            return counter;
        }

        public static event Action<MoveData> OnBlocksMatched;
        private static Match3Result match3Result;
        private static Effect warningEffect;

        public static void SetData(Board board, BoardView boardView, PuzzleQuestData puzzleQuestData)
        {
            if (_questBlockTypes == null) _questBlockTypes = new List<BlockQuestView>();
            _questBlockTypes.Clear();

            _board = board;
            _boardView = boardView;

            AssignQuests(puzzleQuestData);
            IsFinished = false;
            Coupons = 0;
            AddListeners();
            match3Result = null;
            SetLockQuestModules();
        }

        private static void SetLockQuestModules()
        {
            LockQuests = new List<Cell>();
            foreach (var cell in _board.Cells)
            {
                var lockQuest = cell.GetModule<LockQuestModule>();
                if (lockQuest != null)
                {
                    LockQuests.Add(cell);
                }
            }
        }


        private static void AddListeners()
        {
            _board.OnSuccessMove += ReduceAMove;
            _board.OnAddMoves += AddMoves;
            BaseModule.Cleared += OnModuleCleared;
            SwapState.EnterState += CheckFinish;
        }

        internal static int GetMaxCount(string blockID)
        {
            for (int i = 0; i < _questBlockTypes.Count; i++)
            {
                var blockType = _questBlockTypes[i].blockViewData.blockType;
                if (blockType.id.Equals(blockID))
                {
                    return _questBlockTypes[i].MaxCount;
                }
            }
            return 0;
        }

        private static void RemoveListeners()
        {
            Log.Debug("Match3", $"QuestGiver. remove all listeners");
            if (_board != null)
            {
                _board.OnSuccessMove -= ReduceAMove;
                _board.OnAddMoves -= AddMoves;
            }
            BaseModule.Cleared -= OnModuleCleared;
            SwapState.EnterState -= CheckFinish;
        }

        public static void Dispose()
        {
            RemoveListeners();
            LockQuests?.Clear();
            OnBlocksMatched = null;
            OnFinished = null;
        }

        private static void AddMoves(int value)
        {
            CurrentMoves += value;
            IsFinished = false;
            InvokeBlockMatchedEvent();
        }
        private static void ReduceAMove()
        {
            CurrentMoves--;
            InvokeBlockMatchedEvent();

            if (CurrentMoves == 5)
            {
                warningEffect = _boardView.PlayEffect("move-warning", Vector3.zero);
                warningEffect.OnComplete += RecycleWarningEffect;
                BoardView.PlayAudio("5MovesLeft");
            }
        }

        private static void RecycleWarningEffect()
        {
            _boardView.ReleaseEffect(warningEffect);
        }

        private static void AssignQuests(PuzzleQuestData puzzleQuestData)
        {
            CurrentMoves = puzzleQuestData.totalMoves;
            QuestData = new List<QuestData>();
            for (int i = 0; i < puzzleQuestData.questDatas.Count; i++)
            {
                QuestViewData questViewData = puzzleQuestData.questDatas[i];
                QuestData questData = new QuestData { QuestName = questViewData.QuestName, count = questViewData.count };
                QuestData.Add(questData);
                if (questViewData is BlockQuestView blockQuestView) _questBlockTypes.Add(blockQuestView);
            }
            FinishedQuestsInView = new List<string>();
        }

        private static void CheckFinish()
        {
            if (IsFinished) return;
            if (!IsAllQuestsDone())
            {
                if (CurrentMoves == 0)
                {
                    match3Result = new Match3Result()
                    {
                        winStatus = WinStatus.Lose,
                        RemainingQuests = QuestData,
                        RemainingMoves = CurrentMoves,
                        TotalCoupons = Coupons
                    };
                    IsFinished = true;
                    OnFinished?.Invoke(match3Result);
                    Log.Debug("Match3", $"Finished. {match3Result.winStatus} - {CurrentMoves}");
                }
            }
            else
            {
                match3Result = new Match3Result()
                {
                    winStatus = WinStatus.Win,
                    RemainingQuests = QuestData,
                    RemainingMoves = CurrentMoves,
                    TotalCoupons = Coupons
                };

                IsFinished = true;
                OnFinished?.Invoke(match3Result);
                Log.Debug("Match3", $"Finished. {match3Result.winStatus} - {CurrentMoves}");
            }
        }

        static HashSet<Point> allquestPoints = new HashSet<Point>(),
            allGlassPoints = new HashSet<Point>(),
            allIronPoints = new HashSet<Point>(),
            allWoodPoints = new HashSet<Point>(),
            allPlantPoints = new HashSet<Point>(),
            allCandlePoints = new HashSet<Point>(),
            allUnParquetCells = new HashSet<Point>(),
            allLockedPoints = new HashSet<Point>();
        static Dictionary<string, List<Point>> questPoints = new Dictionary<string, List<Point>>();
        private static List<Point> notHitPoints = new List<Point>();
        public static Point GetQuestPos()
        {
            ExtractAllInformation();

            var cells = _board.Cells;

            if (IsInQuest(FlowerIDs.QuestFlower))
            {
                if (questPoints.ContainsKey(BlockIDs.flower))
                {
                    var questPoints = QuestGiver.questPoints[BlockIDs.flower];
                    foreach (var point in questPoints)
                    {
                        Cell cell = _board.Cells[point.x, point.y];
                        BlockModule block = cell.GetModule<BlockModule>();
                        if (block.blockType.id == BlockIDs.flower)
                        {
                            return point;
                        }
                    }
                }
            }

            foreach (var allCandlePoint in allCandlePoints)
            {
                return allCandlePoint;
            }

            foreach (var ironPoint in allIronPoints)
            {
                if (allquestPoints.Contains(ironPoint))
                {
                    //   Debug.LogError("found iron quest at " + ironPoint);
                    return ironPoint;
                }
            }

            foreach (var woodPoint in allWoodPoints)
            {
                if (allquestPoints.Contains(woodPoint))
                {
                    // Debug.LogError("found wood quest at " + woodPoint);
                    return woodPoint;
                }
            }

            foreach (var glassPoint in allGlassPoints)
            {
                //  Debug.LogError("found glass quest at " + glassPoint);
                return glassPoint;
            }

            foreach (var woodPoint in allWoodPoints)
            {
                //  Debug.LogError("found simple wood " );
                return woodPoint;
            }

            if (IsInQuest(BlockIDs.Meter))
            {
                if (questPoints.ContainsKey(BlockIDs.Meter))
                {
                    var questPoints = QuestGiver.questPoints[BlockIDs.Meter];
                    foreach (var point in questPoints)
                    {
                        Cell cell = _board.Cells[point.x, point.y];
                        BlockModule block = cell.GetModule<BlockModule>();
                        if (block.blockType.id == BlockIDs.Meter)
                        {
                            return point;
                        }
                    }
                }
            }

            foreach (var plantPoint in allPlantPoints)
            {
                var point = new Point(plantPoint.x, plantPoint.y - 1);
                while (_board.IsInBounds(point))
                {
                    // Debug.LogError("checking under plant at " + _board.Cells[plantPoint.x, plantPoint.y].position);
                    BlockModule plantModule = _board.Cells[point.x, point.y].GetModule<BlockModule>();
                    Cell cell = _board.Cells[point.x, point.y];
                    if (cell.IsLocked(ActionType.HitBlock, Direction.Center))
                    {
                        break;
                    }
                    if ((plantModule != null && plantModule.id.Equals(BlockIDs.Plant)) ||
                        cell.IsVisible == false)
                    {
                        //   Debug.LogError("either this point is invis or a plant");
                        point.y--;
                        continue;
                    }
                    else
                    {
                        //   Debug.LogError("found point under plant and returning");
                        return point;
                    }
                }
            }

            foreach (var unParquetCell in allUnParquetCells)
            {
                return unParquetCell;
            }

            var leastLeft = "";
            var leastLeftWithJumbo = "";
            var minQuest = 1000;
            var maxJumboIndexDictionary = new Dictionary<string, int>();
            var maxComboNoneJumboIndexDictionary = new Dictionary<string, int>();

            for (int i = 0; i < QuestData.Count; i++)
            {
                if (!QuestData[i].QuestName.Equals(BlockIDs.Plant) && QuestData[i].count > 0 && QuestData[i].count < minQuest &&
                    questPoints.ContainsKey(QuestData[i].QuestName))
                {

                    var noneJumboExists = false;
                    var maxJumbo = -1;
                    var maxCombo = -1;
                    var points = questPoints[QuestData[i].QuestName];
                    for (int j = 0; j < points.Count; j++)
                    {
                        BlockModule blockModule = _board.Cells[points[j].x, points[j].y].GetModule<BlockModule>();
                        if (blockModule != null)
                        {
                            if (!blockModule.IsJumbo)
                            {
                                noneJumboExists = true;
                                if (maxCombo == -1)
                                    maxCombo = j;
                                else if (_board.Cells[points[j].x, points[j].y].GetModule<BlockModule>().Count >
                                         _board.Cells[points[maxCombo].x, points[maxCombo].y].GetModule<BlockModule>()
                                             .Count)
                                    maxCombo = j;
                            }
                            else
                            {
                                if (maxJumbo == -1)
                                    maxJumbo = j;
                                else if (_board.Cells[points[j].x, points[j].y].GetModule<BlockModule>().Count >
                                         _board.Cells[points[maxJumbo].x, points[maxJumbo].y].GetModule<BlockModule>()
                                             .Count)
                                    maxJumbo = j;
                            }
                        }
                    }

                    if (maxJumbo != -1)
                        maxJumboIndexDictionary.Add(QuestData[i].QuestName, maxJumbo);
                    if (maxCombo != -1)
                        maxComboNoneJumboIndexDictionary.Add(QuestData[i].QuestName, maxCombo);

                    if (noneJumboExists)
                    {
                        leastLeft = QuestData[i].QuestName;
                        minQuest = QuestData[i].count;
                    }
                    else
                        leastLeftWithJumbo = QuestData[i].QuestName;
                }
            }

            if (!leastLeft.Equals(""))
            {
                //Debug.LogError("least quest left is " + leastLeft + " highest none jumbo combo is " + questPoints[leastLeft][maxComboNoneJumboIndexDictionary[leastLeft]]);
                return questPoints[leastLeft][maxComboNoneJumboIndexDictionary[leastLeft]];
            }

            if (leastLeftWithJumbo != "")
            {
                // Debug.LogError("least left with jumbo is  " +leastLeftWithJumbo + " highest none jumbo combo is " +  questPoints[leastLeftWithJumbo][maxJumboIndexDictionary[leastLeftWithJumbo]]);
                return questPoints[leastLeftWithJumbo][maxJumboIndexDictionary[leastLeftWithJumbo]];
            }

            foreach (var ironPoint in allIronPoints)
            {
                return ironPoint;
            }

            if (notHitPoints.Count <= 0)
                return Point.None;

            var rand = Random.Range(0, notHitPoints.Count);
            return notHitPoints[rand];
        }

        public static void ExtractAllInformation()
        {
            allquestPoints = new HashSet<Point>();
            int width = _board.Width;
            int height = _board.Height;

            if (notHitPoints == null)
                notHitPoints = new List<Point>();
            else
                notHitPoints.Clear();

            if (questPoints == null)
                questPoints = new Dictionary<string, List<Point>>();
            else
                questPoints.Clear();
            if (allGlassPoints == null)
                allGlassPoints = new HashSet<Point>();
            else
                allGlassPoints.Clear();
            if (allIronPoints == null)
                allIronPoints = new HashSet<Point>();
            else
                allIronPoints.Clear();
            if (allWoodPoints == null)
                allWoodPoints = new HashSet<Point>();
            else
                allWoodPoints.Clear();
            if (allPlantPoints == null)
                allPlantPoints = new HashSet<Point>();
            else
                allPlantPoints.Clear();
            if (allCandlePoints == null)
                allCandlePoints = new HashSet<Point>();
            else
                allCandlePoints.Clear();

            if (allLockedPoints == null)
                allLockedPoints = new HashSet<Point>();
            else
                allLockedPoints.Clear();

            if (allUnParquetCells == null) allUnParquetCells = new HashSet<Point>();
            else allUnParquetCells.Clear();

            bool cacheParquets = false;
            if (IsInQuest("parquet"))
            {
                cacheParquets = true;
            }


            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var cell = _board.Cells[x, y];

                    if (cell.HitType != HitType.None || cell.Contains<LockModule>())
                        continue;

                    for (int i = 0; i < QuestData.Count; i++)
                    {
                        if (QuestData[i].count <= 0)
                            continue;
                        if (cell.HasModuleWithID(QuestData[i].QuestName))
                        {
                            if (!questPoints.ContainsKey(QuestData[i].QuestName))
                                questPoints.Add(QuestData[i].QuestName, new List<Point>());
                            questPoints[QuestData[i].QuestName].Add(cell.position);
                            allquestPoints.Add(cell.position);
                        }
                    }

                    if (cacheParquets)
                        if (cell.CanHaveParquet())
                            allUnParquetCells.Add(cell.position);

                    if (cell.Contains<GlassModule>())
                        allGlassPoints.Add(cell.position);
                    if (cell.Contains<WoodIronModule>() && cell.GetModule<WoodIronModule>().id.Contains("wood"))
                        allWoodPoints.Add(cell.position);
                    if (cell.Contains<WoodIronModule>() && cell.GetModule<WoodIronModule>().id.Contains("iron"))
                        allIronPoints.Add(cell.position);
                    if (cell.HasModuleWithID("plant"))
                        allPlantPoints.Add(cell.position);
                    if (cell.HitType == HitType.None && cell.IsVisible && cell.ClearedBlock == null && !cell.Contains<CandleModule>())
                        notHitPoints.Add(cell.position);
                    if (cell.Contains<CandleModule>() && !cell.GetModule<CandleModule>().active)
                        allCandlePoints.Add(cell.position);

                }
            }

        }

        public static bool IsInQuest(BaseModule module)
        {
            var questName = module.id;
            if (questName == null || questName == string.Empty) return false;
            for (int i = 0; i < QuestData.Count; i++)
            {
                QuestData questData = QuestData[i];
                if (questData.QuestName == questName && questName != "flower")
                {
                    if (questData.count > 0) return true;
                    else return false;
                }
            }
            return false;
        }

        public static bool IsInQuest(string questName)
        {
            if (questName == null || questName == string.Empty) return false;
            for (int i = 0; i < QuestData.Count; i++)
            {
                QuestData questData = QuestData[i];
                if (questData.QuestName == questName && FinishedQuestsInView.Contains(questName) == false)
                {
                    if (questData.count <= 0) FinishedQuestsInView.Add(questName);
                    return true;
                }
            }
            return false;
        }

        private static bool IsAllQuestsDone()
        {
            for (int i = 0; i < QuestData.Count; i++)
            {
                if (QuestData[i].count > 0) return false;
            }

            return true;
        }

        private static void OnModuleCleared(BaseModule baseModule)
        {
            // Log.Debug("Match3", $"Module Cleared Quest done {baseModule.id}");
            int count = 1;
            if (baseModule is BlockModule block)
            {
                count = block.Count;
                if (block.HasCoupon) Coupons++;
                if (block.blockType.id == BlockIDs.Meter) block.id = BlockIDs.Meter;
                if (block.blockType.id == BlockIDs.flower && block.blockType.color == BlockColor.None) block.id = BlockIDs.flower;

                for (int i = 0; i < LockQuests.Count; i++)
                {
                    var lockQuest = LockQuests[i].GetModule<LockQuestModule>();
                    if (lockQuest.color == block.blockType.color)
                    {
                        lockQuest.ReduceCount(block.Count);
                    }
                }
            }
            for (int i = 0; i < QuestData.Count; i++)
            {
                QuestData qd = QuestData[i];
                if (baseModule.id == qd.QuestName)
                {
                    // Log.Debug("Quest", $"Quest done {qd.QuestName}");
                    qd.count -= count;
                    qd.count = Mathf.Max(qd.count, 0);
                }
            }

            InvokeBlockMatchedEvent();
        }

        private static void InvokeBlockMatchedEvent()
        {
            var remainingQuest = new List<QuestData>();
            foreach (var item in QuestData)
            {
                remainingQuest.Add(new QuestData() { count = item.count, QuestName = item.QuestName });
            }
            MoveData data = new MoveData()
            {
                MatchInfos = _board.MatchInfos,
                RemainingMoves = CurrentMoves,
                TotalCoupons = Coupons,
                RemainingQuests = remainingQuest
            };
            OnBlocksMatched?.Invoke(data);
        }

        internal static void OnGUI()
        {
            if (QuestData == null) return;
            _movesPosition = new Rect(10, 10, 200, 100);
            GUI.Label(_movesPosition, "Moves: " + CurrentMoves);
            _movesPosition.y += 20;
            GUI.Label(_movesPosition, "Result: " + (match3Result == null ? "Keep Playing!" : match3Result.winStatus.ToString()));
            _movesPosition.y += 20;
            GUI.Label(_movesPosition, "Coupons: " + Coupons);

            for (int i = 0; i < QuestData.Count; i++)
            {
                QuestData questViewData = QuestData[i];
                GUI.Label(new Rect(_movesPosition.x,
                        _movesPosition.y + (20 * (i + 1)),
                        _movesPosition.width,
                        _movesPosition.height),
                    questViewData.QuestName + " : " + questViewData.count);
            }
        }
    }
}