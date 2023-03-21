using System.Collections.Generic;
using HB.Match3.Cell;
using HB.Match3.Cell.Effect;
using NaughtyAttributes;
using UnityEngine;
namespace HB.Match3.View
{
    [CreateAssetMenu(menuName = "Match3/Layer ViewData", fileName = "new LayerViewData")]
    public class LayerViewData : ScriptableObject
    {
        [ReorderableList] public List<CellTile> allTiles;
        [ReorderableList] public List<GameObject> allPrefabs; 
        [ReorderableList] public List<Effect> effects;
    }
}