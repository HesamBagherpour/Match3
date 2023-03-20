using Garage.Match3.Cells.Modules;
using HB.Match3;
using HB.StateMachine;

namespace Garage.Match3.BoardStates
{
    public class IfMatchFound : Condition
    {
        #region Private Fields

        private readonly Board _board;
        private readonly MatchPredictor _predictor;

        #endregion

        #region  Constructors

        public IfMatchFound(Board board, MatchPredictor predictor)
        {
            _board = board;
            _predictor = predictor;
        }

        #endregion

        #region Public Methods

        public override bool CheckCondition()
        {
            for (int x = 0; x < _board.Width; x++)
            {
                for (int y = 0; y < _board.Height; y++)
                {
                    Cells.Cell cell = _board.Cells[x, y];
                    bool hasPlantInTop = false;
                    if (_board.IsInBounds(x, y + 1))
                    {
                        hasPlantInTop = _board.Cells[x, y + 1].IsRestrictedBlock(ActionType.HitBlock);
                    }
                    bool hasFilledExit = cell.Contains<ExitModule>() && hasPlantInTop;
                    if (_predictor.IsMatch(cell) || hasFilledExit) return true;
                }
            }

            return false;
        }

        #endregion
    }
}