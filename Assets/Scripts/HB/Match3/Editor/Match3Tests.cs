using System.Collections.Generic;
using Garage.Match3;
using Garage.Match3.Cells.Modules;
using HB.Match3.Cells.Modules;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

//using static Garage.Match3.BlockColor;

namespace HB.Match3.Editor
{
    public class Match3BoardDataMissingReferences
    {
        private static List<BoardData> allBoardData = new List<BoardData>();

        [SetUp]
        public void SetUp()
        {
            allBoardData.Clear();
            string[] allLevelGUIDs = AssetDatabase.FindAssets("Match3Level t:BoardData", new[] { "Assets/Match3Levels" });
            foreach (var levelGuid in allLevelGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(levelGuid);
                BoardData boardData = AssetDatabase.LoadAssetAtPath<BoardData>(assetPath);
                allBoardData.Add(boardData);
            }
            Debug.Log($"{allLevelGUIDs.Length} level data was found");
            Assert.IsTrue(allLevelGUIDs.Length != 0);
        }

        [Test]
        public void No_Missing_References_In_BlockTypes()
        {
            // Debug.Log($"No_Missing_References_In_BlockTypes for {allBoardData.Count} levels");
            foreach (var boardData in allBoardData)
            {
                foreach (var blockType in boardData.blockTypes)
                {
                    Assert.IsNotNull(blockType, $"Level {boardData.index} has amissing blocktype reference");
                }
            }
        }

        [Test]
        public void Board_Width_Or_Height_Are_Not_Zero()
        {
            foreach (var boardData in allBoardData)
            {
                Assert.IsTrue(boardData.width != 0 && boardData.height != 0, $"Level {boardData.index} has zero dimentions in width or height");
            }
        }

        [Test]
        public void There_Are_At_Least_One_Quest_For_Each_Level()
        {
            foreach (var boardData in allBoardData)
            {
                Assert.IsTrue(boardData.questData.questDatas.Count != 0, $"Level {boardData.index} has no quest assigned");
            }
        }

        [Test]
        public void There_Are_At_Least_One_Count_For_Each_Level_Quset()
        {
            foreach (var boardData in allBoardData)
            {
                foreach (var questData in boardData.questData.questDatas)
                {
                    if (questData != null) Assert.IsTrue(questData.count != 0, $"Level {boardData.index} has a quest with count = 0");
                }
            }
        }

        [Test]
        public void There_Are_At_Least_One_Move_For_Each_Level()
        {
            foreach (var boardData in allBoardData)
            {
                Assert.IsTrue(boardData.questData.totalMoves != 0, $"Level {boardData.index} no total moves to play");
            }
        }

        [Test]
        public void Quest_Data_References_Are_Not_Missing()
        {
            foreach (var boardData in allBoardData)
            {
                foreach (var questData in boardData.questData.questDatas)
                {
                    Assert.IsTrue(questData != null, $"Level {boardData.index} has a missing reference quest");
                }
            }
        }

        [Test]
        public void BoardTutorialPrefabs_References_Are_Not_Missing()
        {
            foreach (var boardData in allBoardData)
            {
                foreach (var boardTutorialPrefab in boardData.boardTutorialPrefabs)
                {
                    Assert.IsTrue(boardTutorialPrefab != null, $"Level {boardData.index} has a missing board tutorial prefab");
                }
            }
        }

        [Test]
        public void Lock_Quest_Colors_Are_Not_None()
        {
            foreach (var boardData in allBoardData)
            {
                foreach (var lockQuest in boardData.lockQuestPairs)
                {
                    Assert.IsTrue(lockQuest.color != BlockColor.None, $"Level {boardData.index} has a Lock Quest color None");
                }
            }
        }

        [Test]
        public void Lock_Quest_Count_Are_Not_Zero()
        {
            foreach (var boardData in allBoardData)
            {
                foreach (var lockQuest in boardData.lockQuestPairs)
                {
                    Assert.IsTrue(lockQuest.count != 0, $"Level {boardData.index} has a zero count for Lock Quest {lockQuest.color}");
                }
            }
        }

        [Test]
        public void Lock_Quest_Position_Matches_Cell_Position_That_Has_LockQuest_Module()
        {
            foreach (var boardData in allBoardData)
            {
                foreach (var lockQuest in boardData.lockQuestPairs)
                {
                    bool hasACellWithModule = false;
                    boardData.Deserialize();
                    foreach (var cell in boardData.cells)
                    {
                        var lockQuestModule = cell.GetModule<LockQuestModule>();
                        if (lockQuestModule != null && cell.position == lockQuest.position)
                        {
                            hasACellWithModule = true;
                            break;
                        }
                    }
                    boardData.Serialize();
                    Assert.IsTrue(hasACellWithModule, $"Level {boardData.index} has a wrong Lock Quest position: {lockQuest.color}-{lockQuest.position}");
                }
            }
        }

        [Test]
        public void Levels_With_Plant_In_Quest_Has_Exit_Module()
        {
            foreach (var boardData in allBoardData)
            {
                foreach (var quest in boardData.questData.questDatas)
                {
                    if (quest != null && quest.QuestName.Contains(BlockIDs.Plant))
                    {
                        bool hasExitModuleInBoard = false;
                        boardData.Deserialize();
                        foreach (var cell in boardData.cells)
                        {
                            var exitModule = cell.GetModule<ExitModule>();
                            if (exitModule != null)
                            {
                                hasExitModuleInBoard = true;
                                break;
                            }
                        }
                        boardData.Serialize();
                        Assert.IsTrue(hasExitModuleInBoard, $"Level {boardData.index} has a plant quest, but there are no exit modules in board");
                    }
                }
            }
        }

        [Test]
        public void Gnome_Count_In_Quests_Match_Gnome_Count_In_Board()
        {
            foreach (var boardData in allBoardData)
            {
                foreach (var quest in boardData.questData.questDatas)
                {
                    if (quest != null && quest.QuestName.Contains(BlockIDs.Gnome))
                    {
                        int gnomeCountInBoard = 0;
                        boardData.Deserialize();
                        foreach (var cell in boardData.cells)
                        {
                            var blockModule = cell.GetModule<BlockModule>();
                            if (blockModule != null && blockModule.id.Contains(BlockIDs.Gnome))
                            {
                                gnomeCountInBoard++;
                            }
                        }
                        boardData.Serialize();
                        Assert.IsTrue(gnomeCountInBoard == quest.count, $"Level {boardData.index} has {quest.count} gnomes, but there are {gnomeCountInBoard} gnomes in board");
                    }
                }
            }
        }

        [Test]
        public void Possible_Parquet_Count_In_Quests_Match_Parquet_Count_In_Board()
        {
            foreach (var boardData in allBoardData)
            {
                foreach (var quest in boardData.questData.questDatas)
                {
                    if (quest != null && quest.QuestName.Contains(ParquetModule.LayerName))
                    {
                        int parquetCountInBoard = 0;
                        boardData.Deserialize();
                        foreach (var cell in boardData.cells)
                        {
                            if (cell.IsParquetableCell())
                            {
                                parquetCountInBoard++;
                            }
                        }
                        boardData.Serialize();
                        Assert.IsTrue(parquetCountInBoard == quest.count, $"Level {boardData.index} has {quest.count} parquets in quests, but there are {parquetCountInBoard} cells to parquet in board");
                    }
                }
            }
        }

        [Test]
        public void Sofa_Count_In_Quests_Match_Sofa_Count_In_Board()
        {
            foreach (var boardData in allBoardData)
            {
                int sofaCountInQuests = 0;
                bool hasSofa = false;
                foreach (var quest in boardData.questData.questDatas)
                {
                    if (quest != null && quest.QuestName.Contains("sofa"))
                    {
                        boardData.Deserialize();
                        hasSofa = true;
                        sofaCountInQuests += quest.count;
                    }
                }
                if (hasSofa)
                {
                    Assert.IsTrue(sofaCountInQuests == boardData.sofaList.Count, $"Level {boardData.index} has {sofaCountInQuests} sofa in quests, but there are {boardData.sofaList.Count} cells with sofa in board");
                    boardData.Serialize();
                }
            }
        }

        [Test]
        public void Wood_Count_In_Quests_Match_Wood_Count_In_Board()
        {
            foreach (var boardData in allBoardData)
            {
                foreach (var quest in boardData.questData.questDatas)
                {
                    if (quest != null && quest.QuestName.Contains("wood"))
                    {
                        int woodCountInBoard = 0;
                        boardData.Deserialize();
                        foreach (var cell in boardData.cells)
                        {
                            var woodIronModule = cell.GetModule<WoodIronModule>();
                            if (woodIronModule != null && woodIronModule.id.Contains("wood"))
                            {
                                woodCountInBoard++;
                            }
                        }
                        boardData.Serialize();
                        Assert.IsTrue(woodCountInBoard == quest.count, $"Level {boardData.index} has {quest.count} woods inquest, but there are {woodCountInBoard} woods in board");
                    }
                }
            }
        }

        [Test]
        public void Sofas_In_Board_All_Covered_With_Glass()
        {
            foreach (var boardData in allBoardData)
            {
                boardData.Deserialize();
                foreach (var sofaModule in boardData.sofaList)
                {
                    var position = sofaModule.position;
                    var width = sofaModule.size.x;
                    var height = sofaModule.size.y;
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            var cell = boardData.cells[position.x + i + (boardData.width * (position.y - j))];
                            Assert.IsTrue(cell.Contains<GlassModule>(), $"Level {boardData.index} has sofa in {position}, but there are no glass covered at ({position.x + i},{position.y - j}) in board");
                        }
                    }
                }
                boardData.Serialize();
            }
        }
    }

    //    [Category("Match3")]
    //    public class Match3BoardCreationTests
    //    {
    //        Board board;
    //        private int width = 5;
    //        private int height = 10;

    //        [SetUp]
    //        public void SetUp()
    //        {
    //            BoardData data = new BoardData { Width = width, Height = height };
    //            Cell[] cells = new Cell[width * height];
    //            for (int x = 0; x < width; x++)
    //            {
    //                for (int y = 0; y < height; y++)
    //                {
    //                    cells[x + y * width] = new Cell();
    //                }
    //            }
    //            data.Cells = cells;
    //            IBoardView view = Substitute.For<IBoardView>();
    //            board = new Board(data, view);
    //        }

    //        [Test]
    //        public void Match3Board_CreateState()
    //        {
    //            board.Update(0);

    //            Assert.AreEqual(width, board.Width);
    //            Assert.AreEqual(height, board.Height);

    //            Assert.IsNotNull(board.Cells);
    //            Assert.IsNotNull(board.Blocks);

    //            Assert.AreEqual(board.Cells.GetLength(0), width);
    //            Assert.AreEqual(board.Cells.GetLength(1), height);
    //            Assert.AreEqual(board.Blocks.GetLength(0), width);
    //            Assert.AreEqual(board.Blocks.GetLength(1), height);

    //            for (int x = 0; x < board.Width; x++)
    //            {
    //                for (int y = 0; y < board.Height; y++)
    //                {
    //                    Assert.IsNotNull(board.Cells[x, y]);
    //                    Assert.IsNotNull(board.Blocks[x, y]);
    //                }
    //            }
    //        }

    //        [Test]
    //        public void Match3Board_BlockPositionOnCreate()
    //        {
    //            board.Update(0); // Init to create
    //            for (int x = 0; x < width; x++)
    //            {
    //                for (int y = 0; y < height; y++)
    //                {
    //                    Assert.AreEqual(x, board.Blocks[x, y].X);
    //                    Assert.AreEqual(y, board.Blocks[x, y].Y);
    //                }
    //            }
    //        }

    //        [Test]
    //        public void Match3Board_CreateState_HasNoMatch()
    //        {
    //            board.Update(0); // Init to create
    //            for (int x = 0; x < board.Width - 2; x++)
    //            {
    //                for (int y = 0; y < board.Height - 2; y++)
    //                {
    //                    BlockColor bt = board.Blocks[x, y].BlockType;

    //                    Assert.IsFalse(board.Blocks[x + 1, y].BlockType == bt &&
    //                                   board.Blocks[x + 2, y].BlockType == bt,
    //                        $"Block at [{x}, {y}] color = {bt}\n" +
    //                        $"{board.StringizeBlocks()}");
    //                    Assert.IsFalse(board.Blocks[x, y + 1].BlockType == bt &&
    //                                   board.Blocks[x, y + 2].BlockType == bt,
    //                        $"Block at [{x}, {y}] color = {bt}\n" +
    //                        $"{board.StringizeBlocks()}");
    //                }
    //            }
    //        }

    //        [Test]
    //        public void Match3Board_CreateState_HasNoNoneType()
    //        {
    //            board.Update(0); // Init to create
    //            for (int x = 0; x < board.Width - 2; x++)
    //            {
    //                for (int y = 0; y < board.Height - 2; y++)
    //                {
    //                    Assert.AreNotEqual(board.Blocks[x, y].BlockType, None,
    //                        $"None color at [{x}, {y}]");
    //                }
    //            }
    //        }

    //        [Test]
    //        public void Match3Board_CreateState_SetBlocks()
    //        {
    //            ///////////////
    //            // O O O O O //
    //            // O O O O O //
    //            // O O O O O //
    //            // O O O O O //
    //            ///////////////
    //            BlockColor[,] blocks =
    //            {
    //                {None, None, None, None, None},
    //                {None, None, None, None, None},
    //                {None, None, None, None, None},
    //                {None, None, None, None, None},
    //                {None, None, None, None, None},
    //                {None, None, None, None, None}
    //            };
    //            BoardData data = new BoardData
    //            {
    //                Width = blocks.GetLength(0),
    //                Height = blocks.GetLength(1)
    //            };
    //            Cell[] cells = new Cell[data.Width * data.Height];
    //            for (int x = 0; x < data.Width; x++)
    //            {
    //                for (int y = 0; y < data.Height; y++)
    //                {
    //                    cells[x + y * width] = new Cell();
    //                }
    //            }
    //            data.Cells = cells;
    //            IBoardView view = Substitute.For<IBoardView>();
    //            board = new Board(data, view);
    //            board.Update(0); // Init to create
    //            //board.SetBlocks(blocks);
    //            for (int x = 0; x < board.Width; x++)
    //            {
    //                for (int y = 0; y < board.Height; y++)
    //                {
    //                    Assert.AreEqual(board.Blocks[x, y].BlockType, None);
    //                }
    //            }
    //        }
    //    }

    //    [Category("Match3")]
    //    public class Match3SwapTests
    //    {

    //        Board board;
    //        private int width = 5;
    //        private int height = 5;
    //        ///////////////
    //        // O O O O O //
    //        // O O O O O //
    //        // O O O O O //
    //        // O O O O O //
    //        ///////////////
    //        BlockColor[,] blocks =
    //        {
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //        };

    //        [SetUp]
    //        public void SetUp()
    //        {
    //            BoardData data = new BoardData { Width = width, Height = height };
    //            Cell[] cells = new Cell[data.Width * data.Height];
    //            for (int x = 0; x < data.Width; x++)
    //            {
    //                for (int y = 0; y < data.Height; y++)
    //                {
    //                    cells[x+ y * width] = new Cell();
    //                }
    //            }
    //            data.Cells = cells;
    //            IBoardView view = Substitute.For<IBoardView>();
    //            board = new Board(data, view);
    //        }

    //        [Test]
    //        public void Match3Board_SetBlockTypes()
    //        {
    //            board.Update(0);
    //            blocks = new[,]
    //            {   /*x0*/
    //                /*y0*/{ Red, None, None, None, None},
    //                { None, Red, Red, None, None},
    //                { None, None, None, None, None},
    //                { None, None, None, None, None},
    //                { None, None, None, None, None},
    //                { None, None, None, None, None}
    //            };
    //            board.SetBlocks(blocks);

    //        }

    //        [Test]
    //        public void Match3Board_SwapRequest_Vertical()
    //        {
    //            board.Update(0); // Init to create
    //            board.Update(0); // Create to Swap

    //            board.Blocks[0, 0].BlockType = Red;
    //            board.Blocks[0, 1].BlockType = Red;
    //            board.Blocks[0, 2].BlockType = Red;


    //            Block block1 = board.Blocks[0, 0];
    //            block1.Name = "0";
    //            Block block2 = board.Blocks[0, 1];
    //            block2.Name = "1";

    //            Assert.IsTrue(board.SwapBehaviour.Swap(block1, block2) == SwapResponse.Success);
    //            Assert.AreEqual("1", board.Blocks[0, 0].Name);
    //            Assert.AreEqual("0", board.Blocks[0, 1].Name);
    //        }

    //        [Test]
    //        public void Match3Board_SwapRequest_Horizontal()
    //        {
    //            board.Update(0); // Init to create
    //            board.Update(0); // Create to Swap

    //            board.Blocks[0, 0].BlockType = Red;
    //            board.Blocks[1, 0].BlockType = Red;
    //            board.Blocks[2, 0].BlockType = Red;

    //            Block block1 = board.Blocks[0, 0];
    //            block1.Name = "0";
    //            Block block2 = board.Blocks[1, 0];
    //            block2.Name = "1";

    //            Assert.IsTrue(board.SwapBehaviour.Swap(block1, block2) == SwapResponse.Success);
    //            Assert.AreEqual("1", board.Blocks[0, 0].Name);
    //            Assert.AreEqual("0", board.Blocks[1, 0].Name);
    //        }

    //        [Test]
    //        public void Match3Board_Adjacent_ReturnTrue()
    //        {
    //            board.Update(0); // Init to create
    //            Assert.IsTrue(board.IsAdjacent(board.Blocks[0, 0], board.Blocks[0, 1]));
    //            Assert.IsTrue(board.IsAdjacent(board.Blocks[1, 0], board.Blocks[0, 0]));
    //            Assert.IsTrue(board.IsAdjacent(board.Blocks[2, 2], board.Blocks[2, 1]));
    //            Assert.IsTrue(board.IsAdjacent(board.Blocks[1, 1], board.Blocks[1, 0]));
    //        }

    //        [Test]
    //        public void Match3Board_Adjacent_ReturnFalse()
    //        {
    //            board.Update(0); // Init to create
    //            Assert.IsFalse(board.IsAdjacent(board.Blocks[1, 0], board.Blocks[0, 1]));
    //            Assert.IsFalse(board.IsAdjacent(board.Blocks[2, 0], board.Blocks[4, 0]));
    //            Assert.IsFalse(board.IsAdjacent(board.Blocks[0, 0], board.Blocks[1, 1]));
    //            Assert.IsFalse(board.IsAdjacent(board.Blocks[0, 0], board.Blocks[0, 0]));
    //        }

    //        [Test]
    //        public void Match3Board_Swap_Replaced()
    //        {
    //            board.Update(0);
    //            blocks = new[,]
    //            {   /*x0*//*x1*/
    //          /*y0*/{ Red, Blue, None, None, None},
    //          /*y1*/{ None, Red, None, None, None},
    //                { None, Red, None, None, None},
    //                { None, None, None, None, None},
    //                { None, None, None, None, None},
    //            };

    //            board.SetBlocks(blocks);

    //            board.SwapBehaviour.Swap(board.Blocks[0, 0], board.Blocks[0, 1]);
    //            Assert.IsTrue(board.Blocks[0, 1].BlockType == Red);

    //        }

    //        [Test]
    //        public void Match3Board_Swap_WillMatchVertical_RetunTrue()
    //        {
    //            board.Update(0);
    //            blocks = new[,]
    //            {   /*x0*//*x1*/
    //          /*y0*/{ Red, Blue, None, None, None},
    //          /*y1*/{ None, Red, None, None, None},
    //                { None, Red, None, None, None},
    //                { None, None, None, None, None},
    //                { None, None, None, None, None},
    //            };

    //            board.SetBlocks(blocks);
    //            board.StringizeBlocks();
    //            Assert.IsTrue(board.SwapBehaviour.Swap(board.Blocks[0, 0], board.Blocks[0, 1]) == SwapResponse.Success);
    //        }

    //        [Test]
    //        public void Match3Board_Swap_WillMatchVertical_RetunFalse()
    //        {
    //            board.Update(0);
    //            blocks = new[,]
    //            {
    //                { Red, Green, None, None, None},
    //                { None, Red, None, None, None},
    //                { None, Yellow, None, None, None},
    //                { None, None, None, None, None},
    //                { None, None, None, None, None},
    //            };

    //            board.SetBlocks(blocks);
    //            board.SwapBehaviour.Swap(board.Blocks[0, 0], board.Blocks[0, 1]);
    //            Assert.IsFalse(board.SwapBehaviour.Swap(board.Blocks[0, 0], board.Blocks[1, 0]) == SwapResponse.Success);
    //        }

    //        [Test]
    //        public void Match3Board_Swap_WillMatchHorizontal_RetunTrue()
    //        {
    //            board.Update(0);
    //            blocks = new[,]
    //            {
    //                { None, None, None, None, None},
    //                { Red, None, None, None, None},
    //                { None, None, None, None, None},
    //                { Red, None, None, None, None},
    //                { Red, None, None, None, None},
    //            };

    //            board.SetBlocks(blocks);
    //            Assert.IsFalse(board.SwapBehaviour.Swap(board.Blocks[0, 0], board.Blocks[1, 0]) == SwapResponse.Success);
    //        }

    //        [Test]
    //        public void Match3Board_Swap_WillMatchHorizontal2Match_RetunFalse()
    //        {
    //            board.Update(0);
    //            blocks = new[,]
    //            {
    //                { None, None, None, None, None},
    //                { Red, None, None, None, None},
    //                { None, None, None, None, None},
    //                { Red, None, None, None, None},
    //                { None, None, None, None, None},
    //            };

    //            board.SetBlocks(blocks);
    //            board.StringizeBlocks();
    //            board.SwapBehaviour.Swap(board.Blocks[0, 0], board.Blocks[1, 0]);
    //            Assert.IsFalse(board.SwapBehaviour.Swap(board.Blocks[0, 0], board.Blocks[1, 0]) == SwapResponse.Success);
    //        }
    //    }

    //    [Category("Match3")]
    //    public class Match3MatchTests
    //    {
    //        Board board;
    //        private int width = 5;
    //        private int height = 5;
    //        ///////////////
    //        // O O O O O //
    //        // O O O O O //
    //        // O O O O O //
    //        // O O O O O //
    //        ///////////////
    //        BlockColor[,] blocks =
    //        {
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //        };

    //        [SetUp]
    //        public void SetUp()
    //        {
    //            BoardData data = new BoardData()
    //            {
    //                Width = width,
    //                Height = height
    //            };
    //            Cell[] cells = new Cell[data.Width * data.Height];
    //            for (int x = 0; x < data.Width; x++)
    //            {
    //                for (int y = 0; y < data.Height; y++)
    //                {
    //                    cells[x+ y * width] = new Cell();
    //                }
    //            }
    //            data.Cells = cells;
    //            IBoardView view = Substitute.For<IBoardView>();
    //            board = new Board(data, view);
    //            board.Update(0);
    //        }

    //        [Test]
    //        public void Match3Board_MatchHorizontal_3()
    //        {

    //            blocks = new[,]
    //            {
    //                { None, None, None, None, None},
    //                { Red, None, None, None, None},
    //                { Blue, None, None, None, None},
    //                { Red, None, None, None, None},
    //                { Red, None, None, None, None},
    //            };

    //            board.SetBlocks(blocks);
    //            board.StringizeBlocks();
    //            Block block = board.Blocks[1, 0];
    //            Block otherBlock = board.Blocks[2, 0];
    //            board.SwapBehaviour.Swap(block, otherBlock);
    //            board.MatchBehaviour.CheckMatchs();
    //            Assert.AreEqual(MatchType.Horizontal, board.MatchInfos[0].Type);
    //            Assert.AreEqual(2, board.MatchInfos[0].MatchedBlocks[0].X);
    //            Assert.AreEqual(0, board.MatchInfos[0].MatchedBlocks[0].Y);

    //            Assert.AreEqual(4, board.MatchInfos[0].MatchedBlocks[2].X);
    //            Assert.AreEqual(0, board.MatchInfos[0].MatchedBlocks[2].Y);
    //            Assert.AreEqual(3, board.MatchInfos[0].MatchedBlocks.Count);
    //        }

    //        [Test]
    //        public void Match3Board_MatchHorizontal_5()
    //        {
    //            ///////////////
    //            // O O O O O //
    //            // O O O O O //
    //            // O O R O O //
    //            // R R B R R //
    //            ///////////////
    //            blocks = new[,]
    //            {
    //                { Red, None, None, None, None},
    //                { Red, None, None, None, None},
    //                { Blue, Red, None, None, None},
    //                { Red, None, None, None, None},
    //                { Red, None, None, None, None},
    //            };

    //            board.SetBlocks(blocks);
    //            board.StringizeBlocks();
    //            Block block = board.Blocks[2, 1];
    //            Block otherBlock = board.Blocks[2, 0];
    //            board.SwapBehaviour.Swap(block, otherBlock);
    //            board.MatchBehaviour.CheckMatchs();
    //            Assert.AreEqual(MatchType.Horizontal, board.MatchInfos[0].Type);
    //            Assert.AreEqual(0, board.MatchInfos[0].MatchedBlocks[0].X);
    //            Assert.AreEqual(0, board.MatchInfos[0].MatchedBlocks[0].Y);

    //            Assert.AreEqual(4, board.MatchInfos[0].MatchedBlocks[4].X);
    //            Assert.AreEqual(0, board.MatchInfos[0].MatchedBlocks[4].Y);
    //            Assert.AreEqual(5, board.MatchInfos[0].MatchedBlocks.Count);
    //        }
    //    }

    //    [Category("Match3")]
    //    public class Match3FallTests
    //    {
    //        Board board;
    //        private int width = 5;
    //        private int height = 5;
    //        ///////////////
    //        // O O O O O //
    //        // O O O O O //
    //        // O O O O O //
    //        // O O O O O //
    //        ///////////////
    //        BlockColor[,] blocks =
    //        {
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //        };

    //        [SetUp]
    //        public void SetUp()
    //        {
    //            BoardData data = new BoardData { Width = width, Height = height };
    //            Cell[] cells = new Cell[data.Width * data.Height];
    //            for (int x = 0; x < data.Width; x++)
    //            {
    //                for (int y = 0; y < data.Height; y++)
    //                {
    //                    cells[x + y * width] = new Cell { Type = CellType.Empty };
    //                }
    //            }
    //            data.Cells = cells;
    //            IBoardView view = Substitute.For<IBoardView>();
    //            board = new Board(data, view);
    //            board.Update(0);
    //        }

    //        [Test]
    //        public void Match3Board_Fall_ToLowest()
    //        {
    //            ///////////////
    //            // O O R O O //
    //            // O O O O O //
    //            // O O O O O //
    //            // O O O O O //
    //            ///////////////
    //            blocks = new[,]
    //            {
    //                { None, None, None, None, None},
    //                { None, None, None, None, None},
    //                { None, None, None, None,  Red},
    //                { None, None, None, None, None},
    //                { None, None, None, None, None},
    //            };

    //            board.SetBlocks(blocks);
    //            board.StringizeBlocks();
    //            Block block = board.Blocks[2, 4];

    //            board.FallBehaviour.FallBlocks();
    //            Assert.AreEqual(2, block.X);
    //            Assert.AreEqual(0, block.Y);
    //        }
    //    }

    //    [Category("Match3")]
    //    public class Match3PathFall
    //    {
    //        Board board;
    //        private int width = 5;

    //        private int height = 5;

    //        ///////////////
    //        // O O O O O //
    //        // O O O O O //
    //        // O O O O O //
    //        // O O O O O //
    //        ///////////////
    //        BlockColor[,] blocks =
    //        {
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //            {None, None, None, None, None},
    //        };

    //        [SetUp]
    //        public void SetUp()
    //        {
    //            BoardData data = new BoardData { Width = width, Height = height };
    //            Cell[] cells = new Cell[data.Width * data.Height];
    //            for (int x = 0; x < data.Width; x++)
    //            {
    //                for (int y = 0; y < data.Height; y++)
    //                {
    //                    cells[x + y * width] = new Cell { Type = CellType.Empty };
    //                }
    //            }
    //            data.Cells = cells;
    //            IBoardView view = Substitute.For<IBoardView>();
    //            board = new Board(data, view);
    //            board.Update(0);
    //        }

    //        [Test]
    //        public void FallToPath()
    //        {
    //            BlockColor[,] blocks =
    //            {
    //                {None, None, None, None, None},
    //                {None, None, None, None, None},
    //                {None, None, None, None, Red},
    //                {None, None, None, None, None},
    //                {None, None, None, None, None},
    //            };
    //            width = blocks.GetLength(0);
    //            height = blocks.GetLength(1);
    //            CellType[,] cellTypes = {
    //                {CellType.Empty, CellType.Empty, CellType.Empty, CellType.Empty, CellType.Empty},
    //                {CellType.Empty, CellType.Empty, CellType.Empty, CellType.Empty, CellType.Empty},
    //                {CellType.Empty, CellType.Empty, CellType.Empty, CellType.Empty, CellType.Empty},
    //                {CellType.Empty, CellType.Empty, CellType.Empty, CellType.Empty, CellType.Empty},
    //                {CellType.Empty, CellType.Empty, CellType.Empty, CellType.Empty, CellType.Empty},
    //            };
    //            Cell[] cells = new Cell[width * height];
    //            for (int x = 0; x < width; x++)
    //            {
    //                for (int y = 0; y < height; y++)
    //                {
    //                    Cell cell = new Cell { Type = cellTypes[x, y] };
    //                    cells[x + y * width] = cell;
    //                }
    //            }
    //            BoardData data = new BoardData
    //            {
    //                Width = width,
    //                Height = height,
    //                Cells = cells
    //            };
    //            IBoardView view = Substitute.For<IBoardView>();

    //            board = new Board(data, view);
    //            board.SetBlocks(blocks);
    //            board.Path.BakePath();
    //            List<ValueTuple<int, int>> path = board.Path.GetPath(2, 0);
    //            List<ValueTuple<int, int>> expectedPath = new List<ValueTuple<int, int>>();
    //            expectedPath.Add(new ValueTuple<int, int>(2, 0));
    //            expectedPath.Add(new ValueTuple<int, int>(2, 1));
    //            expectedPath.Add(new ValueTuple<int, int>(2, 2));
    //            expectedPath.Add(new ValueTuple<int, int>(2, 3));
    //            expectedPath.Add(new ValueTuple<int, int>(2, 4));

    //            Assert.AreEqual(5, path.Count);
    //            for (int i = 0; i < path.Count; i++)
    //            {
    //                Assert.AreEqual(expectedPath[i], path[i]);
    //            }

    //            List<Block> fallingBlocks = board.FallBehaviour.FallBlocks()[2];
    //            Assert.AreEqual(5, fallingBlocks.Count);
    //            Block block = fallingBlocks[0];
    //            Assert.AreEqual(2, block.X);
    //            Assert.AreEqual(0, block.Y);
    //            Assert.AreEqual(expectedPath.Count, block.Path.Count + 1);
    //            for (int i = 0; i < expectedPath.Count; i++)
    //            {
    //                Assert.AreEqual(expectedPath[i].Item1, path[i].Item1);
    //                Assert.AreEqual(expectedPath[i].Item2, path[i].Item2);
    //            }

    //        }
    //    }

}
