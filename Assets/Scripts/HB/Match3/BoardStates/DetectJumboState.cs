using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using HB.Match3;
using HB.Match3.Cells.Modules;

namespace Garage.Match3.BoardStates
{
    public class DetectJumboState : BoardState
    {
        private bool _finished;

        public List<(Point pos, BlockType blockType, int count)> _cachedJumbos { get; private set; }

        public DetectJumboState()
        {
            _cachedJumbos = new List<(Point pos, BlockType blockType, int count)>();
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            CacheJumbos();
            _finished = true;
        }
        private void CacheJumbos()
        {
            _cachedJumbos.Clear();
            foreach (var boosterInfo in Agent.BoosterInfo)
            {
                if (boosterInfo.Type == BoosterType.Square)
                {
                    var originCell = boosterInfo.OriginCell;
                    var matchedCells = boosterInfo.cells;
                    var block = originCell.GetModule<BlockModule>();
                    if (block == null || QuestGiver.IsInQuest(block.id) == false) continue;
                    int totalCount = 0;
                    for (int i = 0; i < matchedCells.Count; i++)
                    {
                        Cell cell = matchedCells[i];
                        BlockModule otherBlock = cell.GetModule<BlockModule>();
                        if (otherBlock != null)
                        {
                            if (otherBlock.IsJumbo == false) otherBlock.IgnoreQuestBySquareMatch = true;
                            if (otherBlock != null)
                            {
                                totalCount += otherBlock.Count;
                            }
                        }
                    }
                    _cachedJumbos.Add((originCell.position, block.blockType, totalCount));
                }
            }
        }
        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_finished)
            {
                Finished();
            }
        }

    }
}