using HB.Match3.Behaviours;
using HB.Match3.Cell;
using HB.Match3.Modules;
using HB.Match3.View.Quest;
using HB.Packages.Logger;

namespace HB.Match3.Board.BoardStates
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
            //HB //MatchInfoMerger.Merge(Agent.MatchInfos); //HB 
            HitMatchedCells();
            IncreaseAudioChromIndex();
            // Log.Debug("MatchState", "AudioChromIndex: " + AudioChromIndex);
            BoardView.PlayAudio($"Match_chrom_{AudioChromIndex}");
        }

        private void HitMatchedCells()
        {
            
            //HB______________________________________
            for (int i = 0; i < Agent.MatchInfos.Count; i++)
            {
                
                //HB 
                // MatchInfo matchInfo = Agent.MatchInfos[i];
                // for (int j = 0; j < matchInfo.MatchedCells.Count; j++)
                // {
                //     MyCell cell = matchInfo.MatchedCells[j];
                //     cell.Hit(HitType.Direct, 1);
                //     if (IgnoreAdjacent == false) cell.HitAdjacents();
                // }
            }
            //HB_______________________________________
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
                        //HB _______________
                        // if (cell.HitType == HitType.None ||
                        //    (cell.HitType == HitType.Indirect &&
                        //    QuestGiver.IsInQuest(block) &&
                        //    block.Count >= 2 &&
                        //    block.IsJumbo == false))
                        // {
                        //     block.ResetCount();
                        // }
                        //HB _________________
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