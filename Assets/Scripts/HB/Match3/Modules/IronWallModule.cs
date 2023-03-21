
using System;
using HB.Match3.Board;
using HB.Match3.Cell;

namespace HB.Match3.Modules
{
    public class IronWallModule : BaseModule
    { 
        public Restriction Restriction;
        private MyCell neighbourCell;

        public MyCell NeighbourCell => neighbourCell;

        public void SetNeighbour(MyCell cell) => neighbourCell = cell;
        protected RestrictionView _restrictionView;

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            _restrictionView = (RestrictionView)view;
        }

        protected override void OnSetup()
        {
        }

        public override void Clear(MyCell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
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