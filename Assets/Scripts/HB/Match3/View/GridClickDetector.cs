using System;
using Garage.Match3.Cells;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Garage.Match3.View
{
    public class GridClickDetector : MonoBehaviour
    {
        private TilemapSettings _tilemapSetting;
        private static Camera _camera;

        public delegate void GridClick(Point position);
        public static event GridClick OnClick;
        private void LateUpdate()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (IsPointerOverUIObject()) return;
                var pos = GetBoardPoint(Input.GetTouch(0).position);
                OnClick?.Invoke(pos);
            }
            // #elif UNITY_STANDALONE || UNITY_EDITOR
            else if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUIObject()) return;
                var pos = GetBoardPoint(Input.mousePosition);
                OnClick?.Invoke(pos);
            }
#endif
        }

        private bool IsPointerOverUIObject()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return true;

            for (int touchIndex = 0; touchIndex < Input.touchCount; touchIndex++)
            {
                Touch touch = Input.GetTouch(touchIndex);
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return true;
            }

            return false;
        }

        Point GetBoardPoint(Vector2 clickedPosition)
        {
            if (_camera == null) _camera = Camera.main;
            if (_camera == null) _camera = GameObject.FindWithTag("match3_camera").GetComponent<Camera>();
            Vector3 worldPos = _camera.ScreenToWorldPoint(clickedPosition);
            Vector3Int worldCellPos = _tilemapSetting.grid.WorldToCell(worldPos);
            Point position = new Point(worldCellPos.x - _tilemapSetting.boardOffset.x, worldCellPos.y - _tilemapSetting.boardOffset.y);
            // Debug.Log($"Clicked on ClickPosition: {clickedPosition} - ClickToCell: {worldCellPos} - with offset: {position} - BoardOffset: {_tilemapSetting.boardOffset}");
            return position;
        }

        internal void SetData(TilemapSettings tilemapSettings)
        {
            _tilemapSetting = tilemapSettings;
        }

        internal static void SetCamera(Camera puzzleCamera)
        {
            _camera = puzzleCamera;
        }
    }
}