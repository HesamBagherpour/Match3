using HB.Match3.Cell;
using System;
using HB.Audio;
using HB.Match3.Block;
using HB.Match3.Modules;
using HB.Match3.View;

namespace HB.Match3.Board
{
    public interface IBoardView
    {
        bool HasTutorial { get; }
        #region Public Methods
        BoardViewData GetBlockViewData();
        void SetViewData(BoardViewData data, AudioPlayer audioPlayer);
        void SetBoardData(BoardData boardData);
        void Dispose();
        void CreateCellView(MyCell cell);
        void CreateView(BaseModule module, Point position);
        void ShowNextTutorial();
        void Hide();
        void Show(Action onComplete);
        void PlayEffect(Point pos, string id, Action onClear);
        void PlayCollectCounter();
        void PlayBucketBlockEffect(Point startPos, Point targetPos, BlockType blockType, Action onComplete);
        bool IsValidSwap(Point pos, Direction direction);
        void Reshuffle();
        void PlayFingerBoosterEffect();
        void ReleaseFingerBoosterEffect();
        #endregion
    }
    
}