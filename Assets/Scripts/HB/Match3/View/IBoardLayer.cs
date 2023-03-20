using System;
using Garage.Match3.Cells;
using HB.Match3.View;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Garage.Match3.View
{
    public interface IBoardLayer
    {
        int ActiveCells { get; }
        void Dispose();
        void SetTile(int x, int y, IModuleView module);
        void Setup(TilemapSettings tilemapSettings);
        TileBase GetTile(int x, int y);
        void Clear(IModuleView pos);
        Effect PlayEffect(Vector3 pos, Vector3 targetPos, string id);
        Effect PlayEffect(Vector3 pos, string id);
        bool ReleaseEffect(Effect effect);
        Vector3 CellToWorld(Point pos);
        Transform Grid { get; }
        void Hide();
        void Show(Action onComplete);
    }
}