using System;
using Garage.Match3;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3.View;
using MessagePack;
using UnityEngine;

namespace HB.Match3.Cells.Modules
{
    [MessagePackObject]
    public class BlockModule : BaseModule
    {
        public event Action<Direction> SwapRequest;
        public event Action PressRequest;
        public const string LayerName = "Block";
        public const int Order = 4;
        [Key(3)] public BlockType blockType;
        [Key(4)] public ActionType restrictionType;
        [IgnoreMember] public int Count { get; private set; }
        [IgnoreMember] public bool IsJumbo { get; private set; }
        [IgnoreMember] public Point hitPosition { get; private set; }
        [IgnoreMember] public bool IgnoreQuestBySquareMatch { get; set; }
        [IgnoreMember] public bool HasCoupon { get; private set; }

        private BlockModuleView _blockModuleView;
        [IgnoreMember] public bool _mergeIntoOther;


        public BlockModule()
        {
            layerName = LayerName;
            order = Order;
            Count = 1;
            SetCoupon(false);
        }

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            _blockModuleView = (BlockModuleView)view;
            _blockModuleView.SetCount(Count, blockType.color);
            _blockModuleView.SwapRequest -= OnSwapRequest;
            _blockModuleView.SwapRequest += OnSwapRequest;
            _blockModuleView.PressRequest -= OnPressRequest;
            _blockModuleView.PressRequest += OnPressRequest;
            _blockModuleView.SetBlockType(blockType);
            SetCoupon(false);
        }

        public void Setup(BlockType bt)
        {
            blockType = bt;
            hitPosition = Point.None;
            id = blockType.ToString();
            _blockModuleView?.SetModuleData(this);
            IsJumbo = false;
            IgnoreQuestBySquareMatch = false;
            _mergeIntoOther = false;
            ResetCount();
        }

        public void ChangeType(BlockType bt)
        {
            Setup(bt);
            SetView(_blockModuleView);
        }

        internal void SuggestMove(Direction direction, bool move)
        {
            _blockModuleView.SuggestMove(direction, move);
            _blockModuleView.SuggestMove(direction, move);
        }

        protected override void OnSetup()
        {
        }

        internal void MergeIntoOthers()
        {
            _mergeIntoOther = true;
            _blockModuleView.SetMergeIntoOther();
        }

        public void SetCoupon(bool state)
        {
            if (state) Debug.Log("SetCoupon");
            HasCoupon = state;
            _blockModuleView?.SetCoupon(state);
        }

        public void MoveTo(Point cellPosition, Action onFinished, bool sweep = false)
        {
            _blockModuleView.MoveTo(cellPosition, onFinished, sweep);
        }

        public void Fall(Point cellPosition, Action onFinished)
        {
            _blockModuleView.Fall(cellPosition, onFinished);
        }

        public void Reshuffle(Point cellPosition, Action onFinished)
        {
            _blockModuleView.Reshuffle(cellPosition, onFinished);
        }

        public void AddCount(int value)
        {
            Count += value;
            Count = Math.Min(Count, 20);
            _blockModuleView.SetCount(Count, IsJumbo ? blockType.color : BlockColor.None);
        }

        public void ResetCount()
        {
            if (IsJumbo) return;
            Count = 1;
            _blockModuleView?.SetCount(Count, blockType.color);
        }

        internal void PlayMeterEffect(Action OnFinished)
        {
            _blockModuleView.PlayMeterEffect(OnFinished);
        }

        public override void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            hitPosition = cell.position;
            _blockModuleView.SwapRequest -= OnSwapRequest;
            _blockModuleView.PressRequest -= OnPressRequest;
            if (IgnoreQuestBySquareMatch == false && blockType.id != BlockIDs.Box) InvokeClearEvent(this);
            _blockModuleView.IgnoreQuestBySquareMatch = IgnoreQuestBySquareMatch;
            _blockModuleView.Clear(() =>
            {
                onFinished?.Invoke(null);
            });
            SetCoupon(false);
            IgnoreQuestBySquareMatch = false;
            cell.SetClearedBlock(this);
        }

        private void OnSwapRequest(Direction dir)
        {
            SwapRequest?.Invoke(dir);
        }

        private void OnPressRequest()
        {
            PressRequest?.Invoke();
        }

        internal void SetJumbo()
        {
            IsJumbo = true;
            _blockModuleView.SetJumbo();
        }
    }
}