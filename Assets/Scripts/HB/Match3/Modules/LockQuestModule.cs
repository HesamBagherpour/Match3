using System.Collections.Generic;
using HB.Match3.Block;
using HB.Match3.Cell;
using System;

namespace HB.Match3.Modules
{
    public class LockQuestModule : RestrictionModule
    {
        public BlockColor color;
        public int Count { get; private set; }
        public List<MyCell> Cells { get; private set; }
        private LockModuleQuestView _lockQuestModuleView;

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            Cells = new List<MyCell>();
            if (view is LockModuleQuestView lockModuleQuestView)
                _lockQuestModuleView = lockModuleQuestView;
        }
        public override void Clear(MyCell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
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

        public bool AddCell(MyCell cell)
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