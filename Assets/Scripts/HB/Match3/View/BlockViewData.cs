using Garage.Match3;
using UnityEngine;

namespace HB.Match3.View
{
    [CreateAssetMenu]
    public class BlockViewData : ScriptableObject
    {
        public BlockType blockType;
        public ActionType restrictionType;
        public Sprite sprite;
        public BlockViewData nestedBlockType;

    }

    [System.Serializable]
    public class BlockAnimationData
    {
        public GameObject animationPrefab;
        public bool hideMainSprite;
    }
}