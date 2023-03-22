using HB.Packages.StateMachine;

namespace HB.Match3.Board
{
    public class BoardState : State<global::HB.Match3.Match3MainBoard.Board>
    {
        #region Protected Properties
        protected MyCell[,] Cells => Agent.Cells;
        protected int Width => Agent.Width;
        protected int Height => Agent.Height;

        #endregion
    }
}