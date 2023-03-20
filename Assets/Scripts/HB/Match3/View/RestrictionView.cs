using Garage.HomeDesign.Ui_Menus;
using Garage.Match3.BoardEditor;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using HB.Match3;
using HB.Match3.Cells.Modules;
using HB.Match3.View;
using UnityEngine;

namespace Garage.Match3.View
{
    public class InvisibleRestrictionView : RestrictionView
    {
        public InvisibleRestrictionView(BaseModule module) : base(module)
        {
            Visible = false;
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
                var collectCounterEffect = Layer.PlayEffect(Layer.CellToWorld(pos), BoardView.CollectCounterEffectName);
                collectCounterEffect.SetCountAndColor(1, BlockColor.None);

                // Play Collect effect
                var targetPos = Match3GameUi.QuestItemPosition(Id);
                Effect collectEffect = Layer.PlayEffect(Layer.CellToWorld(pos), targetPos, "collect");
                var cellTile = (CellTile)Layer.GetTile(pos.x, pos.y);
                collectEffect.SetSprite(cellTile.questSprite);
            }

            Id = id;
            var effectName = Id.Remove(Id.Length - 1);
            BoardView.PlayAudio(effectName);
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