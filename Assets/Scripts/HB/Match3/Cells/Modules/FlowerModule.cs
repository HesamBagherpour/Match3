using Garage.Match3.View;
using MessagePack;
using System;
using HB.Match3;
using HB.Match3.Cells.Modules;
using HB.Match3.View;

namespace Garage.Match3.Cells.Modules
{
    [MessagePackObject] 
    public class FlowerModule : RestrictionModule 
    {

        [Key(4)] public BlockColor color;
        [IgnoreMember] public bool IsActive { get; private set; }
        private FlowerModuleView _flowerModuleView;

        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            IsActive = false;
            if (view is FlowerModuleView flowerModuleView)
            {
                _flowerModuleView = flowerModuleView;
                _flowerModuleView.SetHealth(color, Restriction.health, false);
            }
        }

        public override void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
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
                _flowerModuleView.SetHealth(color, Restriction.health, IsActive);
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