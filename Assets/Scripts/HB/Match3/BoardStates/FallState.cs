using System;
using HB.Match3.Behaviours;

namespace Garage.Match3.BoardStates
{
    public class FallState : BoardState
    {
        #region Private Fields
        private readonly FallBehaviour _fallBehaviour;
        public static Action FallComplete;
        public static Action Entered;
        #endregion

        #region  Constructors
        public FallState(FallBehaviour behaviour)
        {
            _fallBehaviour = behaviour;
        }
        #endregion

        #region Unity

        #endregion

        #region Protected Methods
        protected override void OnEnter()
        {
            base.OnEnter();
            Agent.CreateNewBoardFlow();
            _fallBehaviour.Fall();
            Entered?.Invoke();
            // Move all blocks to their next empty position untill there is no more empty cells 
        }

        protected override void OnExit()
        {
            base.OnExit();
            FallComplete?.Invoke();
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_fallBehaviour.IsFinished)
            {
                Finished();
            }
        }

        #endregion
    }
}