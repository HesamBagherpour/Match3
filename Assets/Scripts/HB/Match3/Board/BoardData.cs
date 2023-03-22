using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using HB.Core.DI;
using HB.Match3.Block;
using HB.Match3.Cell;
using HB.Match3.Models;
using HB.Match3.Modules;
using HB.Match3.View;

namespace HB.Match3.Board
{
    
    [Serializable]
    public struct LockQuestPair
    {
        public int count;
        public Point position;
        public BlockColor color;
    }
    [Serializable]
    [CreateAssetMenu(menuName = "Match3/BoardData")]
    public class BoardData : ScriptableObject
    {
        #region Public Fields

        public int index;
        public BlockViewData[] blockTypes;
        public int height;
        public int width;
        public Vector2Int boardOffset;
        public PuzzleQuestData questData;
        [NonSerialized] public List<SofaModule> sofaList;
        [NonSerialized] public MyCell[] cells;
        [HideInInspector] public byte[] cellsData;
        [HideInInspector] public string cellDataJson;
        [HideInInspector] public byte[] sofaData;
        [HideInInspector] public string sofaDataJson;
        public List<int> boosterHostCells;
        public uint reward;
        public bool hasBooster;
        public bool isHardLevel;
        public bool useInRandomLevels;
        public int criticalSessionCount;
        public int criticalFinalMoves;
        [Header("PreGame PowerUps")]
        public bool PreGameRainbow = true;
        public bool PreGameStar = true;
        public bool PreGameBlaster = true;
        public bool PreGameJumbo = true;

        [Header("InGame PowerUps")]
        public bool Hammer = true;
        public bool InGameRainbow = true;
        public bool InGameBlaster = true;

        public List<BoardTutorial> boardTutorialPrefabs;

        [Header("LockQuests")]
        public List<LockQuestPair> lockQuestPairs = new List<LockQuestPair>();


        #endregion
        public void Serialize()
        {
            if (cells == null) return;
            boosterHostCells = new List<int>();
            CellData[] data = new CellData[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                SerializableContext context = new SerializableContext(new ModuleFactory());
                IList<BaseModule> modules = cells[i].GetModules();
                for (int m = 0; m < modules.Count; m++)
                {
                    context.AddInstance(modules[m]);
                }
                context.Serialize();
                data[i] = new CellData
                {
                    context = context,
                    position = cells[i].position
                };
                MyCell cell = cells[i];
                if (cell.IsBoosterHost) boosterHostCells.Add(i);
            }

            //Debug.Log(JsonConvert.SerializeObject(data, Formatting.Indented));
            //HBcellDataJson = JsonConvert.SerializeObject(data, Formatting.Indented);
            
            //HBcellsData = MessagePackSerializer.Serialize(data);
            cells = null;

            //HBif (sofaList == null) return;
            //HBSofaModule[] sofas = sofaList.ToArray();


            //HBsofaDataJson = JsonConvert.SerializeObject(sofas);
            
            //HBsofaData = MessagePackSerializer.Serialize(sofas);
            //HBsofaList = null; 
        }

        public void Deserialize(bool withMessagePack = false)
        {
            cells = null;
            CellData[] data;
            
            if(!withMessagePack)
                //HBdata = JsonConvert.DeserializeObject<CellData[]>(cellDataJson, new BaseModuleJsonConverter());
                //HBelse
                //data = MessagePackSerializer.Deserialize<CellData[]>(cellsData);
            
                //HB
            // cells = new MyCell[data.Length];
            // for (int i = 0; i < data.Length; i++)
            // {
            //     var d = data[i];
            //     d.context.Deserialize();
            //     cells[i] = new MyCell(d);
            // }
            //HB


            
            if (sofaData == null || sofaData.Length == 0) return;
                sofaList = null;
            //HB
            // if(!withMessagePack)
            //     //sofaList = JsonConvert.DeserializeObject<SofaModule[]>(sofaDataJson, new BaseModuleJsonConverter()).ToList();
            // else
            //     //sofaList = MessagePackSerializer.Deserialize<SofaModule[]>(sofaData).ToList();
            //HB
            if (boosterHostCells != null)
            {
                foreach (var boosterHost in boosterHostCells)
                {
                    cells[boosterHost].IsBoosterHost = true;
                }
            }
        }
    }
}