using System;
using Garage.Match3.Cells;
using HB.Match3;
using UnityEngine;

namespace Garage.Match3.View
{
    public interface IBlockView
    {
        #region Public Properties
        #endregion

        #region Public Methods

        void SetType(BlockType bt);
        void MoveTo(Vector3 pos, Action onMoveFinished, bool bounce);
        void Clear(Action onClearFinished);
        void SetCount(int count);
        void SetJumbo();

        #endregion
    }
}