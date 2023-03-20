using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.Cells.Modules.Boosters;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.Behaviours;
using HB.Match3.Cells.Modules;
using UnityEngine;

namespace Garage.Match3.BoardStates
{
    public class PostClearState : BoardState
    {
        private IBoardView _boardView;
        private BlockFactory _blockFactory;
        private int _postClearCounter;
        private bool _finished;
        public static bool UpdateNeeded { get; private set; }

        public PostClearState(IBoardView boardView, BlockFactory blockFactory)
        {
            _boardView = boardView;
            _blockFactory = blockFactory;
        }
        protected override void OnEnter()
        {
            base.OnEnter();
            _postClearCounter = 0;
            UpdateNeeded = false;
            CheckForPostClear();
        }
        protected override void OnExit()
        {
            base.OnExit();
            _boardView.PlayCollectCounter();
        }

        private void CheckForPostClear()
        {
            for (int i = 0; i < Agent.MatchInfos.Count; i++)
            {
                for (int j = 0; j < Agent.MatchInfos[i].MatchedCells.Count; j++)
                {
                    if (Agent.MatchInfos[i].matchType != MatchType.Horizontal &&
                       Agent.MatchInfos[i].matchType != MatchType.Vertical &&
                       Agent.MatchInfos[i].matchType != MatchType.Square)
                        continue;
                    Cell cell = Agent.MatchInfos[i].MatchedCells[j];
                    if (cell.IsVisible && cell.HitType != HitType.None)
                    {
                        BlockModule block = cell.ClearedBlock;
                        if (block != null)
                        {
                            if (block.blockType.id.Equals(BlockIDs.Simple) || block.blockType.id.Equals(BlockIDs.Box))
                            {
                                for (int index = 0; index < cell.Adjacents.Count; index++)
                                {
                                    var adj = cell.Adjacents[index];
                                    if (adj.ClearedBlock != null && adj.ClearedBlock.blockType.id.Equals(BlockIDs.Nalbaki))
                                    {
                                        var blockType = new BlockType
                                        {
                                            id = BlockIDs.Simple,
                                            color = block.blockType.color
                                        };
                                        _blockFactory.CreateBlock(adj, blockType);
                                    }
                                }
                            }
                        }
                    }
                }
            }



            int width = Agent.Width;
            int height = Agent.Height;




            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = Cells[x, y];
                    if (cell.IsVisible && cell.HitType != HitType.None)
                    {
                        BlockModule block = cell.ClearedBlock;
                        if (block != null)
                        {
                            if (block.id.Contains(BlockIDs.Box) && !block._mergeIntoOther)
                            {
                                _blockFactory.CreateBlock(cell, Agent.GetNestedBlockType(block.blockType));
                            }
                            else if (block.IsJumbo)
                            {
                                _postClearCounter++;
                                JumboBooster jumboBehaviour = new JumboBooster(Agent, cell.position, 1);
                                jumboBehaviour.Activate();
                                _boardView.PlayEffect(cell.position, block.id + "-jumbo", OnPostClearComplete);
                                UpdateNeeded = true;
                            }


                            cell.SetClearedBlock(null);
                        }

                        block = cell.GetModule<BlockModule>();

                        if (block != null && block.id.Equals(BlockIDs.Gnome))
                        {
                            Debug.LogError("found gnome at " + cell.position);
                            if (cell.GetTopRestriction() == null)
                            {
                                Debug.LogError("found gnome with no restriction" + cell.position);
                                var dmg = 1;
                                cell.ClearBlock(ref dmg, HitType.Direct, null);
                            }
                            else
                            {
                                Debug.LogError("gnome has rest " + cell.GetTopRestriction().id + " with health " + cell.GetTopRestriction().Restriction.health);
                            }

                        }
                        cell.SetClearedBlock(null);

                    }
                    else if (cell.GetModule<BlockModule>() != null && cell.GetModule<BlockModule>().id.Equals(BlockIDs.Gnome))
                    {
                        Debug.LogError("gnome is not hit at " + cell.position);
                    }
                }
            }






            if (_postClearCounter == 0)
            {
                _finished = true;
            }
        }

        void OnPostClearComplete()
        {
            _postClearCounter--;
            if (_postClearCounter == 0)
            {
                _finished = true;
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_finished)
            {
                Finished();
                _finished = false;
            }
        }
    }
}