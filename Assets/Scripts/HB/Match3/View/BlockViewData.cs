using HB.Match3.Block;
using HB.Match3.Cell;
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
}