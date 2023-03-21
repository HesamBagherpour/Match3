using UnityEngine;
namespace HB.Match3.View
{
 
        [CreateAssetMenu(fileName = "Board view data", order = 0, menuName = "Match3/Board view Data")]
        public class BoardViewData : ScriptableObject
        {
            public LayerViewData layerViewData;
            public BlockViewDataBase blockViewData;
        }
}