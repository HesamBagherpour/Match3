using System;
using HB.Match3.Board;

namespace HB.Match3.Tilemap
{
    public interface ICustomTilemap
    {
        void Clear();
        void SetAllTiles(BoardData boardData);
        void Hide();
        void Show(Action onComplete);
    }
}