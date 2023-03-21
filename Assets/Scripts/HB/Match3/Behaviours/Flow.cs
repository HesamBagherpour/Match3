#define Fall_Debug
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HB.Match3.Cell;
using HB.Match3.Modules;
using HB.Packages.Logger;
using HB.Packages.Utilities;

namespace HB.Match3.Behaviours
{
    public class Flow
    {
        private static Color[] RandomColors =
        {
            Color.cyan, Color.green, Color.magenta,
            Color.yellow, Color.red, Color.blue,
            Color.black, Color.white,
        };
        public List<(MyCell provider, MyCell startCell)> Incommings = new List<(MyCell provider, MyCell startCell)>();

        private static int colorIndex = 0;
        public List<MyCell> Cells { get; private set; }
        // public Cell Provider;
        // public Cell StartCell;
        public readonly List<Flow> subFlows;
        private Color _color;
        const float offset = 0.5f;


        public Flow(MyCell cell)
        {
            Cells = new List<MyCell>();
            subFlows = new List<Flow>();
            Incommings = new List<(MyCell provider, MyCell startCell)>();
            colorIndex++;
            _color = RandomColors[colorIndex % RandomColors.Length];
        }


        public void AddCell(MyCell cell)
        {
#if Fall_Debug
            if (Cells.Contains(cell))
                Log.Error("Match3",
                    $"A cell at {cell.position} was already added");
#endif
            Cells.Add(cell);
            cell.flow = this;
        }

        public void AddFlow(Flow flow)
        {
            flow._color = _color;
            subFlows.Add(flow);
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder($"<color=#{(byte)_color.r * 255:X2}{(byte)_color.g * 255:X2}{(byte)_color.b * 255:X2}> => ");
            for (int i = 0; i < Cells.Count; i++)
            {
                sb.Append(Cells[i].position);
            }
            sb.Append("</color>");
            return sb.ToString();
        }

        internal MyCell GetCellProvider(MyCell cell, bool includeProvider)
        {
            var index = Cells.FindIndex(x => x == cell);
            MyCell topCell = null;
            for (int i = index - 1; i >= 0; i--)
            {
                MyCell providercell = Cells[i];
                if (providercell.IsVisible)
                {
                    topCell = providercell;
                    break;
                }
            }
            if (topCell != null && topCell.Contains<BlockModule>())
                return topCell;

            if (includeProvider)
            {
                foreach (var (provider, startCell) in Incommings)
                {
                    int starterCellIndex = Cells.FindIndex(x => x == startCell);
                    if (index == starterCellIndex)
                    {
                        return provider;
                    }
                }
            }
            return null;
        }
#if Fall_Debug
        public void Visualize()
        {
            DrawProvider();
            for (int i = 0; i < Cells.Count - 1; i++)
            {
                Point pos = Cells[i].position;
                Vector3 start = CellToWorld(pos);
                pos = Cells[i + 1].position;
                Vector3 end = CellToWorld(pos);
                DrawArrow.ForDebug(start, end - start, _color);
            }
        }

        private void DrawProvider()
        {
            if (Cells.Count == 0) return;
            if (Incommings.Count == 0) return;
            foreach (var incomming in Incommings)
            {
                Point pos = incomming.provider.position;
                Vector3 start = CellToWorld(pos);
                pos = incomming.startCell.position;
                Vector3 end = CellToWorld(pos);
                DrawArrow.ForDebug(start, end - start, _color);
            }
        }

        private Vector3 CellToWorld(Point pos)
        {
            return new Vector3();
            //HB //return new Vector3(pos.x + Board.Offset.x + offset, pos.y + Board.Offset.y + offset); //HB 
        }

#endif
    }
}