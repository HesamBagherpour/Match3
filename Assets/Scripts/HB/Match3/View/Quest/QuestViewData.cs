using UnityEngine;

namespace Garage.Match3.View.Quest
{
    public abstract class QuestViewData : ScriptableObject
    {
        public abstract string QuestName { get; }
        public abstract Sprite QuestSprite { get; }
        public int count;
    }

    public class QuestData
    {
        public string QuestName { get; set; }
        public int count;
    }
}