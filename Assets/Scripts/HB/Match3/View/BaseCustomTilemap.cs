using System;
using DG.Tweening;
using Garage.Match3.View;
using UnityEngine.Tilemaps;

namespace HB.Match3.View
{
    public abstract class BaseCustomTilemap : ICustomTilemap
    {
        protected Tilemap _tileMap;

        public virtual void Clear()
        {
            _tileMap.ClearAllTiles();
        }
        public abstract void SetAllTiles(BoardData boardData);

        public void Hide()
        {
            var _tileMapColor = _tileMap.color;
            _tileMapColor.a = 0;
            _tileMap.color = _tileMapColor;
        }

        public void Show(Action onComplete)
        {
            var _tileMapColor = _tileMap.color;
            _tileMapColor.a = 1;
            DOTween.To(() => _tileMap.color, x => _tileMap.color = x, _tileMapColor, 1f).SetOptions(true);
        }
    }
}