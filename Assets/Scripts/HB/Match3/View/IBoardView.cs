using System;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using HB.Core.Modules.Audio;
using HB.Match3;
using HB.Match3.Cells.Modules;
using HB.Match3.View;

namespace Garage.Match3.View
{
    public interface IBoardView
    {
        bool HasTutorial { get; }
        #region Public Methods
        BoardViewData GetBlockViewData();
        void SetViewData(BoardViewData data, AudioPlayer audioPlayer);
        void SetBoardData(BoardData boardData);
        void Dispose();
        void CreateCellView(Cell cell);
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