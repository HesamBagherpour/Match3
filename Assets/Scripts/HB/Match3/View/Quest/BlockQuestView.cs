using HB.Match3.View;
using UnityEngine;

namespace Garage.Match3.View.Quest
{
    [CreateAssetMenu]
    public class BlockQuestView : QuestViewData
    {
        public BlockViewData blockViewData;
        public int MaxCount = 0;
        public override string QuestName => blockViewData.blockType.ToString();
        public override Sprite QuestSprite => blockViewData.sprite;
    }
}