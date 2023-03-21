using System.Collections.Generic;
using HB.Match3.Block;
using UnityEngine;

namespace HB.Match3.View
{
    [CreateAssetMenu(menuName = "Match3/Block ViewData ", fileName = "new BlockViewData")]
    public class BlockViewDataBase : ScriptableObject
    {
        #region Public Fields

        public List<BlockViewData> blockViewDatas;

        #endregion

        #region Public Methods

        public Dictionary<BlockType, Sprite> ToDictionary()
        {
            Dictionary<BlockType, Sprite> dict = new Dictionary<BlockType, Sprite>();
            for (int i = 0; i < blockViewDatas.Count; i++)
                dict.Add(blockViewDatas[i].blockType, blockViewDatas[i].sprite);

            return dict;
        }

        public BlockType[] GetBlockTypes()
        {
            List<BlockType> blockTypes = new List<BlockType>();
            for (int i = 0; i < blockViewDatas.Count; i++)
            {
                BlockType blockType = blockViewDatas[i].blockType;
                if (blockType.id != "None") blockTypes.Add(blockType);
            }

            return blockTypes.ToArray();
        }

        #endregion
    }
}