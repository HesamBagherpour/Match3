using System;
using System.Collections.Generic;
using Garage.Match3.BoardStates;
using Garage.Match3.Cells.Modules.Boosters;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.Cells.Modules;
using HB.Match3.View;
using MessagePack;
using UnityEngine;

namespace Garage.Match3.Cells.Modules
{
    [MessagePackObject]
    public class BoosterModule : BaseModule
    {
        public const string LayerName = "BoosterView";
        public const int Order = 3;
        [Key(3)] public BoosterType BoosterType { get; set; }
        [IgnoreMember] public bool IsActive { get; internal set; }

        private BoosterModuleView _boosterView;
        private Booster _boosterBehaviour;
        private Board _board;
        public BoosterModule()
        {
            layerName = LayerName;
            order = Order;
            id = layerName;
        }

        public void SetBoard(Board board)
        {
            _board = board;
        }

        public void SetData(BoosterType type, Point pos, bool playAudio = true)
        {
            BoosterType = type;
            id = type.ToString().ToLower();
            AssignBoosterType(type, pos, playAudio);
        }

        private void AssignBoosterType(BoosterType type, Point pos, bool playAudio)
        {
            switch (type)
            {
                case BoosterType.Horizontal:
                    _boosterBehaviour = new HorizontalBooster(_board, pos, 1);
                    if (playAudio) BoardView.PlayAudio("BoosterApear");
                    break;
                case BoosterType.Vertical:
                    _boosterBehaviour = new VerticalBooster(_board, pos, 1);
                    if (playAudio) BoardView.PlayAudio("BoosterApear");
                    break;
                case BoosterType.Square:
                    _boosterBehaviour = new SquareBooster(_board, pos, 1);
                    if (playAudio) BoardView.PlayAudio("StarApear");
                    break;
                case BoosterType.Cross:
                    _boosterBehaviour = new CrossBooster(_board, pos, 1);
                    if (playAudio) BoardView.PlayAudio("BoosterApear");
                    break;
                case BoosterType.Rainbow:
                    _boosterBehaviour = new RainbowBooster(_board, pos, 1);
                    if (playAudio) BoardView.PlayAudio("RainbowAppear");
                    break;
                case BoosterType.Explosion:
                case BoosterType.JumboBlock:
                    _boosterBehaviour = new JumboBooster(_board, pos, 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            _boosterView?.SetType(BoosterType);
        }

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            _boosterView = (BoosterModuleView)view;
            // _boosterView.SetType(BoosterType);
        }

        public void UpdateBoosterType(BoosterType boosterType, Point pos)
        {
            switch (boosterType)
            {
                case BoosterType.Horizontal:
                    if (BoosterType == BoosterType.Horizontal)
                    {
                        Debug.Log($"Switch booster hitted by {boosterType} - current: {BoosterType} - result: {BoosterType.Vertical}");
                        SetData(BoosterType.Vertical, pos);
                    }
                    break;
                case BoosterType.Vertical:
                    if (BoosterType == BoosterType.Vertical)
                    {
                        Debug.Log($"Switch booster hitted by {boosterType} - current: {BoosterType} - result: {BoosterType.Horizontal}");
                        SetData(BoosterType.Horizontal, pos);
                    }
                    break;
            }
        }

        protected override void OnSetup()
        {
        }

        public override void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            if (damage == 0)
            {
                onFinished?.Invoke(null);
                return;
            }
            _boosterBehaviour.Activate();
            _boosterView.SetBlockType(_boosterBehaviour.GetBlockType());

            switch (BoosterType)
            {
                case BoosterType.None:
                    break;
                case BoosterType.Horizontal:
                case BoosterType.Vertical:
                case BoosterType.Cross:
                    BoardView.PlayAudio("LaiserBusterAction");
                    break;
                case BoosterType.Square:
                    BoardView.PlayAudio("StarBusterAction");
                    break;
                case BoosterType.Rainbow:
                    BoardView.PlayAudio("RainbowAction");
                    break;
                case BoosterType.Explosion:
                    BoardView.PlayAudio("RainbowAction");
                    break;
                default:
                    break;
            }

            List<Point> targetPositions = _boosterBehaviour.GetCellPositions();

            _boosterView.Hit(_boosterBehaviour.TrailID, cell.position, targetPositions, () =>
            {
                BoardView.PlayAudio($"Match_chrom_{MatchState.AudioChromIndex}");
                onFinished?.Invoke(this);
            });
            IsActive = true;
        }
    }

    public enum BoosterType
    {
        None,
        Horizontal,
        Vertical,
        Square,
        Cross,
        Rainbow,
        Explosion,
        Hammer,
        JumboBlock,
    }
}