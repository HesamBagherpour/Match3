using Garage.Match3.BoardEditor;
using UnityEngine;

namespace Garage.Match3.View.Quest
{
    [CreateAssetMenu]
    public class CellQuestView : QuestViewData
    {
        public CellTile cellTile;
        public override string QuestName => cellTile.name;
        public override Sprite QuestSprite => cellTile.questSprite;
    }
}