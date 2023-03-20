using System;
using System.Collections.Generic;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.Cells.Modules;
using HB.Match3.View;
using MessagePack;

namespace Garage.Match3.Cells.Modules
{
    [MessagePackObject]
    public class IronWallModule : BaseModule
    {
        [Key(3)] public Restriction Restriction;
        private Cell neighbourCell;

        [IgnoreMember]public Cell NeighbourCell => neighbourCell;

        public void SetNeighbour(Cell cell) => neighbourCell = cell;
        protected RestrictionView _restrictionView;

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            _restrictionView = (RestrictionView)view;
        }

        protected override void OnSetup()
        {
        }

        public override void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            if (neighbourCell.HitType != HitType.Direct ||
                cell.HitType != HitType.Direct)
            {
                onFinished?.Invoke(null);
                return;
            }

            InvokeClearEvent(this);
            Restriction.health -= damage;
            if (Restriction.health < 0)
            {
                Restriction.health = 0;
            }
            BoardView.PlayAudio(Restriction.health > 0 ? "WallBreak" : "WallDestroy");
            id = id.Remove(id.Length - 1);
            id += Restriction.health.ToString();
            _restrictionView.SetHealth(cell.position, id, Restriction.health);
            _restrictionView.PlayEffect(cell.position, "iron");

            if (Restriction.health == 0)
            {
                onFinished?.Invoke(this);
            }
            else
            {
                onFinished?.Invoke(null);
            }
        }
    }
}