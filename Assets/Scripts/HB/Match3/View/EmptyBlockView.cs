using System;
using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.View;
using UnityEngine;

namespace HB.Match3.View
{
    public class EmptyBlockView : IBlockView
    {
        #region Public Properties

        public Point Pos { get; private set; }

        #endregion

        #region IBlockView Interface

        public void SetType(BlockType bt)
        {
        }

        public void SetPosition(Point pos)
        {
            this.Pos = pos;
        }

        public void MoveTo(Vector3 pos, Action onMoveFinished, bool bounce)
        {
            onMoveFinished?.Invoke();
        }

        public void MergeAndClear((int x, int y) pos, Action onFinished)
        {
            onFinished?.Invoke();
        }

        public void Clear(Action onClearFinished)
        {
            onClearFinished?.Invoke();
        }

        public void MoveTo(List<(int, int)> path, Action moveCallback, Action onFirstSteoFinished)
        {
            Debug.LogError("MOVING EMPTY BLOCK");
            moveCallback?.Invoke();
            onFirstSteoFinished?.Invoke();
        }

        public void SetCount(int count)
        {
        }

        public void SetJumbo()
        {
        }

        #endregion
    }
}