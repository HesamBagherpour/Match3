using System;
using HB.Match3.Behaviours;
using HB.Match3.Board.BoardStates;
using HB.Match3.Cell;
using HB.Match3.Modules;
using HB.Packages.Logger;
using HB.Packages.Utilities;

namespace HB.Match3.Board
{
    public class SwapState : BoardState
    {
       #region Private Fields

        private readonly SwapBehaviour _swapBehaviour;
        private readonly IBoardView _boardView;
        private int _moveCount;
        private SwapData _swapData;
        private bool discardSuggest;
        private const float SwapTimeOut = 3f;
        private float _elapsed = 0;
        private bool _hasFingerBooster;
        private bool _finished;

        public static bool PassedWithRainbowPowerUp { get; internal set; }

        public static event Action ExitState;
        public static event Action EnterState;
        public static event Action DiscardSuggest;

        #endregion

        #region  Constructors

        public SwapState(SwapBehaviour swapBehaviour, IBoardView boardView)
        {
            _swapBehaviour = swapBehaviour;
            _boardView = boardView;
        }

        #endregion

        #region Protected Methods

        protected override void OnEnter()
        {
            base.OnEnter();
            EnterState?.Invoke();
            _moveCount = -1;
            Agent.LastValidSwap.cell = null;
            Agent.LastValidSwap.otherCell = null;
            ListenToSwapAndSuggestIfNon();
            _elapsed = 0;
            _hasFingerBooster = false;
        }

        private void ListenToSwapAndSuggestIfNon()
        {
            _finished = false;
            discardSuggest = false;
            DiscardSuggest?.Invoke();
            MyCell.SwapRequest += OnSwapRequest;
            GridClickDetector.OnClick += OnPressRequest;
            _elapsed = 0;
        }

        private void SuggestSwap()
        {
            if (discardSuggest == false && Board.IsActive && _boardView.HasTutorial == false)
            {
                PossibleMatch possibleMatch = Agent.PossibleMatch;
                if (possibleMatch != null)
                {
                    // Log.Debug("Match3", possibleMatch.ToString());
                    BlockModule blockModule = possibleMatch.mainCell.GetModule<BlockModule>();
                    blockModule.SuggestMove(possibleMatch.direction, true);
                    foreach (var _otherCell in possibleMatch.otherCells)
                    {
                        BlockModule otherBlock = _otherCell.GetModule<BlockModule>();
                        otherBlock.SuggestMove(possibleMatch.direction, false);
                    }
                }
            }
        }


        protected override void OnExit()
        {
            base.OnExit();
            if (PassedWithRainbowPowerUp == false) MatchState.CommingFromSwap = true;
            PassedWithRainbowPowerUp = false;
            ExitState?.Invoke();
            MyCell.SwapRequest -= OnSwapRequest;
            GridClickDetector.OnClick -= OnPressRequest;
        }

        #endregion

        #region Private Methods

        private void OnSwapRequest(Point pos, Direction direction)
        {
            if (_boardView.IsValidSwap(pos, direction) == false) return;
            if (_hasFingerBooster) return;
            discardSuggest = true;
            DiscardSuggest?.Invoke();
            MyCell.SwapRequest -= OnSwapRequest;
            MyCell cell = Agent.Cells[pos.x, pos.y];
            _swapData = new SwapData
            {
                cell = cell,
                direction = direction
            };

            _swapBehaviour.Swap(_swapData, OnSuccess, OnFail);
        }

        private void OnPressRequest(Point pos)
        {
            if (Agent.IsInBounds(pos))
            {
                discardSuggest = true;
                DiscardSuggest?.Invoke();
                MyCell cell = Agent.Cells[pos.x, pos.y];
                if (cell.IsVisible && Agent.HasFingerBooster())
                {
                    _hasFingerBooster = true;
                    Agent.ExecuteFingerBooster(cell, () =>
                     {
                         _moveCount = 0;
                         _finished = true;
                         Log.Debug("Match3", $"ExecuteFingerBooster");
                     });
                }
            }
        }


        private void OnSuccess()
        {
            _finished = true;
            BoardView.PlayAudio("Switch");
            Agent.ReduceAMove();
            MoveBlocks(_swapData.cell, _swapData.otherCell, OnSuccessfulMovesFinished, true);
            _swapData.cell.ExchangeBlocks(_swapData.otherCell);
        }

        private void OnFail(SwapResponse swapResponse)
        {
            switch (swapResponse)
            {
                case SwapResponse.IgnoredBlockID:
                case SwapResponse.NotAdjacent:
                case SwapResponse.InvalidBlock:
                case SwapResponse.Restricted:
                case SwapResponse.InvalidDirection:
                    ListenToSwapAndSuggestIfNon();
                    break;
                case SwapResponse.WontMatch:
                    BoardView.PlayAudio("WrongSwitch");
                    MoveBlocks(_swapData.cell, _swapData.otherCell, OnFailMovesFinished);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(swapResponse), swapResponse, null);
            }

            Agent.LastValidSwap.cell = null;
            Agent.LastValidSwap.otherCell = null;
        }

        private void MoveBlocks(MyCell cell, MyCell otherCell, Action callback, bool sweep = false)
        {
            _moveCount = 0;
            BlockModule block = cell.GetModule<BlockModule>();
            BlockModule otherBlock = otherCell.GetModule<BlockModule>();
            if (block != null)
            {
                _moveCount++;
                block.MoveTo(otherCell.position, callback, sweep);
            }

            if (otherBlock != null)
            {
                _moveCount++;
                otherBlock.MoveTo(cell.position, callback);
            }
        }

        private void ResetMoves(MyCell cell, MyCell otherCell, Action callback)
        {
            _moveCount = 0;
            BlockModule block = cell.GetModule<BlockModule>();
            BlockModule otherBlock = otherCell.GetModule<BlockModule>();
            if (block != null)
            {
                _moveCount++;
                block.MoveTo(cell.position, callback);
            }

            if (otherBlock != null)
            {
                _moveCount++;
                otherBlock.MoveTo(otherCell.position, callback);
            }
        }


        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_moveCount == 0 && _finished == true)
            {
                Finished();
            }
            if (IsFinished == false && discardSuggest == false)
            {
                _elapsed += deltaTime;
                if (_elapsed >= SwapTimeOut)
                {
                    SuggestSwap();
                    _elapsed = 0;
                }
            }
            else
            {
                _elapsed = 0;
            }
        }


        private void OnSuccessfulMovesFinished()
        {
            _moveCount--;
        }

        private void OnFailMovesFinished()
        {
            _moveCount--;
            if (_moveCount == 0)
            {
                ResetMoves(_swapData.otherCell, _swapData.cell, OnBackToPosition);
            }
        }

        private void OnBackToPosition()
        {
            _moveCount--;
            if (_moveCount == 0)
            {
                ListenToSwapAndSuggestIfNon();
            }
        }

        #endregion
    }
    
    
}