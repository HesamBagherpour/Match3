using System;
using HB.Match3.Cell;
using HB.Match3.Cell.Effect;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HB.Match3.Modules
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