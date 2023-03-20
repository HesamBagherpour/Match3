using System;
using Garage.Match3.View;
using HB.Match3;
using HB.Match3.Cells.Modules;
using MessagePack;

namespace Garage.Match3.Cells.Modules
{
    [MessagePackObject] public class GlassModule : RestrictionModule { }
    [MessagePackObject] public class WoodIronModule : RestrictionModule { }
    [MessagePackObject] public class GrassModule : RestrictionModule { }

    [MessagePackObject]
    public class InvisibleBlocker : RestrictionModule
    {
        protected override void OnSetup()
        {
            layerName = VisibleModule.LayerName;
            order = VisibleModule.Order;
        }
    }

    [MessagePackObject]
    public class RestrictionModule : BaseModule
    {
        [Key(3)] public Restriction Restriction;
        private RestrictionView _restrictionView;
        public override void SetView(IModuleView view)
        {
            base.SetView(view);
            if (view is RestrictionView restrictionView)
                _restrictionView = restrictionView;
        }

        protected override void OnSetup()
        {
        }

        public override void Clear(Cell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
        {
            if ((hitType & Restriction.hitType) == hitType)
            {
                int remainingDmg = CalculateRemainingDamage(damage);
                if (Restriction.health < 0)
                {
                    Restriction.health = 0;
                }
                id = id.Remove(id.Length - 1);
                id += Restriction.health.ToString();
                _restrictionView.SetHealth(cell.position, id, Restriction.health);
                damage = remainingDmg;
            }

            if (Restriction.health == 0)
            {
                id = id.Remove(id.Length - 1);
                id += Restriction.health + 1;
                InvokeClearEvent(this);
                onFinished?.Invoke(this);
            }
            else
            {
                onFinished?.Invoke(null);
            }
        }

        private int CalculateRemainingDamage(int damage)
        {
            if (Restriction.Contains(ActionType.HitBlock, Direction.Center))
            {
                int returnedDamage = damage;
                returnedDamage -= Restriction.health;
                if (returnedDamage < 0) returnedDamage = 0;
                Restriction.health -= damage;
                return returnedDamage;
            }
            else
            {
                Restriction.health -= damage;
            }
            return damage;
        }
    }
}