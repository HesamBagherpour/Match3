using Garage.Match3.Cells.Modules;
using UnityEngine;

namespace Garage.Match3.BoardEditor
{
    [CreateAssetMenu(fileName = "New BoosterTile", menuName = "Match3/Tiles/Booster Tile")]
    public class BoosterTile : GameObjectTile
    {
        public BoosterType boosterType;
    }
}