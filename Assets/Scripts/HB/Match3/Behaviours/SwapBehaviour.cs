using HB.Match3.Cell;
using System;
using HB.Match3.Modules;

namespace HB.Match3.Behaviours
{
    public class SwapBehaviour
    {
        #region Private Fields

        private Match3MainBoard.Board _board;
        private readonly MatchPredictor _predictor;
        private const Direction ValidDirection = Direction.Left | Direction.Right | Direction.Top | Direction.Bottom;

        #endregion

        #region  Constructors

        //HB
        public SwapBehaviour(Match3MainBoard.Board board, MatchPredictor predictor)
        {
            _board = board;
            _predictor = predictor;
        }

        //HB
        #endregion

        #region Public Methods

        public void Swap(SwapData data, Action onSuccess, Action<SwapResponse> onFail)
        {
            MyCell cell = data.cell;
            Point targetPos = GetTargetCoords(cell.position, data.direction);
            MyCell otherCell = null;

            if (!_board.IsInBounds(targetPos) || !_board.HasEmptyCell(targetPos))
            {
                onFail?.Invoke(SwapResponse.InvalidBlock);
                return;
            }

            if (ValidDirection.HasFlag(data.direction) == false || data.direction == Direction.None)
            {
                onFail?.Invoke(SwapResponse.InvalidDirection);
                return;
            }

            otherCell = _board.Cells[targetPos.x, targetPos.y];
            if (cell.IsLocked(ActionType.Swap, data.direction) ||
                otherCell.IsLocked(ActionType.Swap, GetOppositeDirection(data.direction)) ||
                cell.Contains<CannonModule>() ||
                otherCell.Contains<CannonModule>())
            {
                onFail?.Invoke(SwapResponse.Restricted);
                return;
            }

            data.otherCell = otherCell;


            SwapResponse response = IsAValidSwap(cell, otherCell);

            if (response == SwapResponse.Success)
            {
                _board.LastValidSwap.cell = data.cell;
                _board.LastValidSwap.otherCell = data.otherCell;
                onSuccess?.Invoke();
            }
            else
            {
                onFail?.Invoke(response);
            }
        }

        private Direction GetOppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Top: return Direction.Bottom;
                case Direction.Left: return Direction.Right;
                case Direction.Bottom: return Direction.Top;
                case Direction.Right: return Direction.Left;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        #endregion

        #region Private Methods

        private static Point GetTargetCoords(Point point, Direction dir)
        {
            int dx = point.x;
            int dy = point.y;
            switch (dir)
            {
                case Direction.Left:
                    dx--;
                    break;
                case Direction.Right:
                    dx++;
                    break;
                case Direction.Top:
                    dy++;
                    break;
                case Direction.Bottom:
                    dy--;
                    break;
                case Direction.None:
                case Direction.Center:
                case Direction.All:
                    return point;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
            }

            return new Point(dx, dy);
        }

        private SwapResponse IsAValidSwap(MyCell cell, MyCell otherCell)
        {
            BlockModule blockModule = cell.GetModule<BlockModule>();
            BlockModule otherBlockModule = otherCell.GetModule<BlockModule>();
            if ((blockModule != null && _board.IsIgnoredBlockType(blockModule.blockType)) ||
                (otherBlockModule != null && _board.IsIgnoredBlockType(otherBlockModule.blockType)))
                return SwapResponse.IgnoredBlockID;

            if (_board.IsAdjacent(cell.position, otherCell.position) == false)
                return SwapResponse.NotAdjacent;

            if (_predictor.WillMatchBySwap(cell, otherCell) ||
                _predictor.WillMatchBySwap(otherCell, cell))
                return SwapResponse.Success;

            return SwapResponse.WontMatch;
        }

        #endregion
    }
    
    
    public enum SwapResponse
    {
        Success,
        InvalidBlock,
        IgnoredBlockID,
        NotAdjacent,
        WontMatch,
        Restricted,
        InvalidDirection
    }
    
    public class SwapData
    {
        public MyCell cell;
        public MyCell otherCell;
        public Direction direction;
    }
}
