using System;
using HB.Match3.Block;
using HB.Match3.Board;
using HB.Match3.Cell;

namespace HB.Match3.Modules
{
    public class BucketModule : RestrictionModule
    {
        public BlockColor color;
        public bool IsActive { get; private set; }
        private BucketModuleView _bucketModuleView;

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            IsActive = false;
            if (view is BucketModuleView bucketModuleView)
            {
                _bucketModuleView = bucketModuleView;
                _bucketModuleView.SetHealth(color, Restriction.health, false);
            }
        }

        public override void Clear(MyCell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            if ((hitType & Restriction.hitType) == hitType)
            {
                Restriction.health++;
                if (Restriction.health > 3)
                {
                    Restriction.health = 1;
                    IsActive = true;
                    BoardView.PlayAudio("bucket-active");
                }
                else
                {
                    BoardView.PlayAudio("bucket-fill");
                }
                id = id.Remove(id.Length - 1);
                id += Restriction.health.ToString();
                _bucketModuleView.SetHealth(color, Restriction.health, IsActive);
                damage = 0;
            }
            onFinished?.Invoke(null);
        }

        internal void BurstComplete()
        {
            IsActive = false;
        }
    }
}