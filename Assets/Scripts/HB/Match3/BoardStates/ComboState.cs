using System;

namespace Garage.Match3.BoardStates
{
    public class ComboState : BoardState
    {
        private PuzzleQuestData _questData;

        public ComboState(PuzzleQuestData questData)
        {
            this._questData = questData;
        }

        #region Protected Methods
        protected override void OnEnter()
        {
            base.OnEnter();
            ClearCombos();
            CheckCombos();
        }
        private void ClearCombos()
        {
            //var width = Agent.Width;
            //var height = Agent.Height;
            //for (int i = 0; i < width; i++)
            //{
            //    for (int j = 0; j < height; j++)
            //    {
            //        (int x, int y) pos = (i, j);
            //        var block = Agent.Blocks[i, j];
            //        if (Agent.HasEmptyCell(pos) && block != Block.Empty && _questData.HasBlockType(block.BlockType) == true && IsMatched(pos) == false && block.Jumbo == false)
            //        {
            //            block.SetCount(1);
            //        }
            //    }
            //}
        }

        private bool IsMatched((int x, int y) pos)
        {
            //var matchInfo = Agent.MatchInfos;
            //for (int i = 0; i < matchInfo.Count; i++)
            //{
            //    var blockList = matchInfo[i].MatchedPoses;
            //    for (int j = 0; j < blockList.Count; j++)
            //    {
            //        if (blockList[j] == pos) return true;
            //    }
            //}
            return false;
        }

        private void CheckCombos()
        {
            //var matchInfos = Agent.MatchInfos;
            //for (int i = 0; i < matchInfos.Count; i++)
            //{
            //    var matchInfo = matchInfos[i];
            //    for (int j = 0; j < matchInfo.MatchedPoses.Count; j++)
            //    {
            //        var pos = matchInfo.MatchedPoses[j];
            //        var block = Agent.Blocks[pos.x, pos.y];
            //        if (block != Block.Empty && Agent.HasEmptyCell(pos) && matchInfo.Type != MatchType.Booster)
            //        {
            //            (int x, int y) topNeighbour = (pos.x, pos.y + 1);
            //            if (Agent.IsInBounds(topNeighbour))
            //            {
            //                Block topBlock = Agent.Blocks[topNeighbour.x, topNeighbour.y];
            //                if (topBlock != Block.Empty && Agent.HasEmptyCell(topNeighbour) && _questData.HasBlockType(topBlock.BlockType) && IsMatched(topNeighbour) == false) topBlock.SetCount(topBlock.Count + 1);
            //            }
            //            (int x, int y) bottomNeighbour = (pos.x, pos.y - 1);
            //            if (Agent.IsInBounds(bottomNeighbour))
            //            {
            //                Block bottomBlock = Agent.Blocks[bottomNeighbour.x, bottomNeighbour.y];
            //                if (bottomBlock != Block.Empty && Agent.HasEmptyCell(bottomNeighbour) && _questData.HasBlockType(bottomBlock.BlockType) && IsMatched(bottomNeighbour) == false) bottomBlock.SetCount(bottomBlock.Count + 1);
            //            }
            //            (int x, int y) leftNeighbour = (pos.x - 1, pos.y);
            //            if (Agent.IsInBounds(leftNeighbour))
            //            {
            //                Block leftBlock = Agent.Blocks[leftNeighbour.x, leftNeighbour.y];
            //                if (leftBlock != Block.Empty && Agent.HasEmptyCell(leftNeighbour) && _questData.HasBlockType(leftBlock.BlockType) && IsMatched(leftNeighbour) == false) leftBlock.SetCount(leftBlock.Count + 1);
            //            }
            //            (int x, int y) rightNeighbour = (pos.x + 1, pos.y);
            //            if (Agent.IsInBounds(rightNeighbour))
            //            {
            //                Block rightBlock = Agent.Blocks[rightNeighbour.x, rightNeighbour.y];
            //                if (rightBlock != Block.Empty && Agent.HasEmptyCell(rightNeighbour) && _questData.HasBlockType(rightBlock.BlockType) && IsMatched(rightNeighbour) == false) rightBlock.SetCount(rightBlock.Count + 1);
            //            }
            //        }
            //    }
            //}
            Finished();
        }

        #endregion
    }
}