using System.Collections.Generic;
using Garage.Match3.BoardEditor;
using Garage.Match3.View;
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