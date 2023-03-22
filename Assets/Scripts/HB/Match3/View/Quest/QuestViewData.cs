using UnityEngine;

namespace HB.Match3.View.Quest
{
    public abstract class QuestViewData : ScriptableObject
    {
        public abstract string QuestName { get; }
        public abstract Sprite QuestSprite { get; }
        public int count;
    }
}