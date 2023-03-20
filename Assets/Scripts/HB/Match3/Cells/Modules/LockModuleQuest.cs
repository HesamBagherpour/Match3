using System;
using System.Collections.Generic;
using Garage.Match3;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using MessagePack;

namespace HB.Match3.Cells.Modules
{
    [MessagePackObject]
    public class LockQuestModule : RestrictionModule
    {
        [Key(4)] public BlockColor color;
        [IgnoreMember] public int Count { get; private set; }
        [IgnoreMember] public List<Cell> Cells { get; private set; }
        private LockModuleQuestView _lockQuestModuleView;

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            Cells = new List<Cell>();
            if (view is LockModuleQuestView lockModuleQuestView)
                _lockQuestModuleView = lockModuleQuestView;
        }
        public override void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            if (damage == 10)
            {
                Restriction.health = 0;
                id = id.Remove(id.Length - 1);
                id += Restriction.health.ToString();
                _lockQuestModuleView.Clear(() => onFinished?.Invoke(this));
                damage = 0;
            }
            else
            {
                onFinished?.Invoke(null);
            }
        }

        public bool AddCell(Cell cell)
        {
            if (Cells.Contains(cell) == false)
            {
                Cells.Add(cell);
                return true;
            }
            return false;
        }

        public void SetInitialCount(int count)
        {
            Count = count;
            _lockQuestModuleView.SetCount(Count);
        }

        public void SetColor(BlockColor color)
        {
            this.color = color;
            _lockQuestModuleView.SetColor(color);
        }

        internal int GetCellCount()
        {
            return Cells.Count;
        }

        internal void ReduceCount(int count)
        {
            Count -= count;
            if (Count < 0) Count = 0;
        }

        internal void PlayCollectEffect()
        {
            _lockQuestModuleView.SetCount(Count);
            _lockQuestModuleView.PlayCollectEffect();
        }
    }
}