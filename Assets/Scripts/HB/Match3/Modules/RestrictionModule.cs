using System;
using HB.Match3.Cell;
using HB.Match3.Cell.Effect;
using HB.Match3.View;
using HB.Match3.View.Quest;
using UnityEngine;

namespace HB.Match3.Modules
{
    public class RestrictionModule :BaseModule
    {
        public Restriction Restriction;
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

        public override void Clear(MyCell cell, ref int damage, HitType hitType, Action<BaseModule> onFinished)
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
    
    public class RestrictionView : ModuleView
    {
        private Sprite _sprite;

        public RestrictionView(BaseModule module) : base(module)
        {
            Visible = true;
        }

        public virtual void SetHealth(Point pos, string id, int health)
        {
            if (health == 0 && QuestGiver.IsInQuest(Id))
            {
                // Play Collect Counter effect
                
                // HB 
     
                // var collectCounterEffect = Layer.PlayEffect(Layer.CellToWorld(pos), BoardView.CollectCounterEffectName);
                // collectCounterEffect.SetCountAndColor(1, BlockColor.None);
                //
                // // Play Collect effect
                // var targetPos = Match3GameUi.QuestItemPosition(Id);
                // Effect collectEffect = Layer.PlayEffect(Layer.CellToWorld(pos), targetPos, "collect");
                // var cellTile = (CellTile)Layer.GetTile(pos.x, pos.y);
                // collectEffect.SetSprite(cellTile.questSprite);
                
                //HB
            }

            Id = id;
            var effectName = Id.Remove(Id.Length - 1);
            //BoardView.PlayAudio(effectName);HB
            PlayEffect(pos, effectName);
            if (health <= 0)
            {
                Id = string.Empty;
            }
            Layer.SetTile(pos.x, pos.y, this);
        }

        public void PlayEffect(Point pos, string effectName)
        {
            Layer.PlayEffect(Layer.CellToWorld(pos), effectName);
        }
    }
}