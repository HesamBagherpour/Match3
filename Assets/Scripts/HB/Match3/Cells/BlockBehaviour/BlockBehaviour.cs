using System;
using HB.Match3;

namespace Garage.Match3.Cells.BlockBehaviour
{
    public interface IBlockBehaviour
    {
        Board Board { get; set; }
        void Execute(Action OnFinish);
    }
    public abstract class BlockBehaviour : IBlockBehaviour
    {
        public Board Board { get; set; }
        public BlockBehaviour(Board board)
        {
            this.Board = board;
        }

        public abstract void Execute(Action OnFinish);
    }
}