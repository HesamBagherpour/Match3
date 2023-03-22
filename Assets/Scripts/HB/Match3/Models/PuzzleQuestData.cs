using System;
using System.Collections.Generic;
using HB.Match3.View.Quest;

namespace HB.Match3.Models
{
    [Serializable]
    public class PuzzleQuestData
    {
        #region Public Fields
        public List<QuestViewData> questDatas;
        public int totalMoves;
        #endregion
    }
}