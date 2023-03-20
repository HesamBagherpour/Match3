using System;
using System.Collections.Generic;
using Garage.Match3.Cells;
using Garage.Match3.View;
using HB.Match3;

namespace Garage.Match3
{
    public class Block
    {
        #region Public Fields
        public Point Pos { get; private set; }
        public int Count { get; private set; }

        #endregion

        #region Private Fields

        private readonly IBlockView _blockView;
        private BlockType _blockType;
        private List<(int, int)> _path = new List<(int, int)>();
        #endregion

        #region Public Properties
        public int PathLenght { get { return _path.Count; } }
        public BlockType BlockType
        {
            get => _blockType;
            set
            {
                _blockType = value;
                _blockView.SetType(_blockType);
            }
        }

        public bool Jumbo { get; private set; }
        #endregion

        #region  Constructors

        public Block(IBlockView blockView)
        {
            Count = 1;
            _blockView = blockView;
            _blockView.SetCount(Count);
        }

        #endregion

        #region Public Methods
        public void SetCount(int count)
        {
            this.Count = count;
            _blockView.SetCount(this.Count);
        }
        //public (int, int) GetEndPath()
        //{
        //    if (_path.Count != 0) return _path[_path.Count - 1];
        //    return Pos;
        //}
        public void SetPath(List<(int, int)> freeCells)
        {
            _path.Clear();
            for (int i = 0; i < freeCells.Count; i++) _path.Add(freeCells[i]);
        }

        public void SetPosition(int x, int y)
        {
            Pos =new Point(x,y);
        }

        public void MoveTo((int x, int y) pos, Action moveCallback = null, bool bounce = false)
        {
            //_blockView.MoveTo(pos, moveCallback, bounce);
        }

        // public void Fall(Action moveCallback = null, Action firstStepCallback = null)
        // {
        //     if (_path != null && _path.Count != 0) _blockView.MoveTo(_path, moveCallback, firstStepCallback);
        //     else moveCallback?.Invoke();
        // }

        public override string ToString()
        {
            return $"[{Pos.x}, {Pos.y}]{_blockType}";
        }

        public void Clear(Action onFinished, float delay = 0.2f)
        {
            //_blockView.MergeAndClear(ox, oy, onFinished);
            _blockView.Clear(onFinished);
        }

        public void SetJumbo(int jumboCount)
        {
            Jumbo = true;
            SetCount(jumboCount);
            _blockView.SetJumbo();
        }

        #endregion

        #region Private Methods
        #endregion
    }
}