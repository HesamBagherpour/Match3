using Garage.Match3.Cells;
using HB.Match3;
using HB.StateMachine;

namespace Garage.Match3.BoardStates
{
    public class BoardState : State<Board>
    {
        #region Protected Properties
        protected Cell[,] Cells => Agent.Cells;
        protected int Width => Agent.Width;
        protected int Height => Agent.Height;

        #endregion
    }
}