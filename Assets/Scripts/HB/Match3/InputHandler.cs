using System;
using Garage.Match3;

namespace HB.Match3
{
    public class InputHandler
    {
        #region Events

        public static event Action<Block, Direction> SwapBlock;

        #endregion

        #region Private Methods

        private static void OnSwapBlock(Block arg1, Direction arg2)
        {
            SwapBlock?.Invoke(arg1, arg2);
        }

        #endregion
    }
}