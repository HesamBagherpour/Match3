using System;
using Garage.Match3.Cells;
using Garage.Match3.View;
using HB.Match3.Behaviours;

namespace Garage.Match3.BoardStates
{
    public class CreateState : BoardState
    {
        #region Private Fields

        private readonly CreateBehaviour _createBehaviour;
        private readonly IBoardView _view;
        private int _finished;

        #endregion

        #region  Constructors

        public CreateState(CreateBehaviour createBehaviour, IBoardView view)
        {
            _createBehaviour = createBehaviour;
            _view = view;
            _finished = 0;
        }

        #endregion

        #region Protected Methods

        protected override void OnEnter()
        {
            base.OnEnter();
            _finished = -1;
            _createBehaviour.CreateBoard(CreateBoardFinished);
        }

        private void CreateBoardFinished()
        {
            _finished = 1;
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            if (_finished == 1)
            {
                Finished();
            }
        }

        #endregion

        #region Private Methods

        #endregion
    }
}