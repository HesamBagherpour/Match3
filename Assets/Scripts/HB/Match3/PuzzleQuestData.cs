using System;
using System.Collections.Generic;
using Garage.Match3.View.Quest;

namespace Garage.Match3
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