using System;
using HB.Match3;

namespace Garage.Match3.View
{
    public interface ICustomTilemap
    {
        void Clear();
        void SetAllTiles(BoardData boardData);
        void Hide();
        void Show(Action onComplete);
    }
}