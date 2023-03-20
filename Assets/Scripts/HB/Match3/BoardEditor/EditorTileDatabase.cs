using System.Collections.Generic;
using Garage.Match3.BoardEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Match3
{
    public class EditorTileDatabase : MonoBehaviour
    {
        public TileBase cell;
        public TileBase spawner;
        public List<BlockTile> blocks = new List<BlockTile>();
        public List<Garage.Match3.BoardEditor.CellTile> sofas = new List<Garage.Match3.BoardEditor.CellTile>();
        public List<Garage.Match3.BoardEditor.CellTile> glasses = new List<Garage.Match3.BoardEditor.CellTile>();
        public List<Garage.Match3.BoardEditor.CellTile> woods = new List<Garage.Match3.BoardEditor.CellTile>();
    }
}