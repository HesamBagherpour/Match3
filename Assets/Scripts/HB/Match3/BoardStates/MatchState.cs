using System;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Logger;
using HB.Match3;
using HB.Match3.Behaviours;
using HB.Match3.Cells.Modules;
using HB.Match3.View;

namespace Garage.Match3.BoardStates
{
    public class MatchState : BoardState
    {
        #region Private Fields

        private readonly MatchBehaviour _matchBehaviour;
        private readonly IBoardView _boardView;
        public static int AudioChromIndex { get; private set; }

        public static bool CommingFromSwap { get; set; }
        public static bool IgnoreAdjacent { get; internal set; }

        private bool _finished;

        #endregion

        #region  Constructors

        public MatchState(MatchBehaviour matchBehaviour, IBoardView boardView)
        {
            _matchBehaviour = matchBehaviour;
            _boardView = boardView;
            AudioChromIndex = 0;
        }

        #endregion

        #region Protected Methods

        protected override void OnEnter()
        {
            base.OnEnter();
            _finished = false;
            if (CommingFromSwap) AudioChromIndex = 0;
            _matchBehaviour.CheckMatches(OnSuccess, OnFailed);
            ResetCombos();
            CommingFromSwap = false;
        }

        #endregion

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_finished)
            {
                Finished();
                _finished = false;
            }
        }
        #region Private Methods
        private void OnSuccess()
        {
            // Log.Debug("MatchState", "OnSuccess");
            MatchInfoMerger.Merge(Agent.MatchInfos);
            HitMatchedCells();
            IncreaseAudioChromIndex();
            // Log.Debug("MatchState", "AudioChromIndex: " + AudioChromIndex);
            BoardView.PlayAudio($"Match_chrom_{AudioChromIndex}");
        }

        private void HitMatchedCells()
        {
            for (int i = 0; i < Agent.MatchInfos.Count; i++)
            {
                MatchInfo matchInfo = Agent.MatchInfos[i];
                for (int j = 0; j < matchInfo.MatchedCells.Count; j++)
                {
                    Cell cell = matchInfo.MatchedCells[j];
                    cell.Hit(HitType.Direct, 1);
                    if (IgnoreAdjacent == false) cell.HitAdjacents();
                }
            }
            IgnoreAdjacent = false;
            _finished = true;
        }

        private void ResetCombos()
        {
            Log.Debug("Match3", $"Reseting combos in MatchState comming from {CommingFromSwap}");
            if (CommingFromSwap == false) return;
            foreach (var cell in Agent.Cells)
            {
                if (cell.IsVisible && cell.IsLocked(ActionType.HitBlock, Direction.Center) == false)
                {
                    BlockModule block = cell.GetModule<BlockModule>();
                    if (block != null)
                    {
                        if (cell.HitType == HitType.None ||
                           (cell.HitType == HitType.Indirect &&
                           QuestGiver.IsInQuest(block) &&
                           block.Count >= 2 &&
                           block.IsJumbo == false))
                        {
                            block.ResetCount();
                        }
                    }
                }
            }
        }

        private void OnFailed()
        {
            Log.Debug("MatchState", "OnFailed");
            _finished = true;
        }

        internal static void IncreaseAudioChromIndex()
        {
            AudioChromIndex++;
            if (AudioChromIndex > 7) AudioChromIndex = 7;
        }

        #endregion
    }
}