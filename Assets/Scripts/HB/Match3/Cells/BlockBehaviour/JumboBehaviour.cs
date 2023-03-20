using System;
using Garage.Match3.Cells.Modules.Boosters;
using HB.Match3;

namespace Garage.Match3.Cells.BlockBehaviour
{
    public class JumboBehaviour : BlockBehaviour
    {
        private Point position;

        public JumboBehaviour(Board board, Point position) : base(board)
        {
            this.position = position;
        }
        public override void Execute(Action OnFinish)
        {
            var jumboBehaviour = new JumboBooster(Board, position, 1);
            jumboBehaviour.Activate();
        }
    }
}