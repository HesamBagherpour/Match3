using System;
using System.Collections.Generic;
using Garage.Match3.BoardStates;
using Garage.Match3.Cells;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HB.Match3
{
    public class BoardTutorial : MonoBehaviour
    {
        private const float CharacterDialogBounceDurarion = 0.4f;
        private const float CharacterDialogBouncePower = 1.5f;

        public struct ValidMove
        {
            public Point Position;
            public Direction Direction;
        }
        public Action OnHide;
        public bool HideByFall = true;
        private Tilemap _tileMap;
        private Grid _grid;
        List<ValidMove> validMoves;
        private Vector2Int _boardOffset;
        private bool _fallCompleted;
        private bool _swapStarted;
        public string AdjustEvent;
        public string AnalaticsEvent;
        private void Awake()
        {
            _grid = GetComponentInChildren<Grid>();
            _tileMap = GetComponentInChildren<Tilemap>();
            validMoves = new List<ValidMove>();
            _swapStarted = false;
            _fallCompleted = false;
        }

        public void Hide()
        {
            if (HideByFall)
            {
                if (_swapStarted && _fallCompleted)
                {
                    // Debug.Log("Hiding tutorial");
                    OnHide?.Invoke();
                    OnHide = null;
                    _swapStarted = false;
                    _fallCompleted = false;
                }
            }
            else
            {
                // Debug.Log("Hiding tutorial");
                OnHide?.Invoke();
                OnHide = null;
            }
        }

        private void OnEnable()
        {
            if (HideByFall)
            {
                FallState.FallComplete += FallComplete;
                SwapState.ExitState += TemporaryHideTutorial;
                SwapState.EnterState += SwapStarted;
            }
        }

        private void TemporaryHideTutorial()
        {
            transform.localScale = Vector3.zero;
        }

        private void SwapStarted()
        {
            _swapStarted = true;
            Hide();
        }

        private void FallComplete()
        {
            _fallCompleted = true;
            _swapStarted = false;
            Hide();
        }

        private void OnDisable()
        {
            if (HideByFall)
            {
                SwapState.EnterState -= SwapStarted;
                SwapState.ExitState -= TemporaryHideTutorial;
                FallState.FallComplete -= FallComplete;
            }
        }
        public void SetData(Vector2Int boardOffset, int width, int height)
        {
            //AdjustWrapper.SendAdjustEvent(AdjustEvent);
            if (AnalaticsEvent != null && !AnalaticsEvent.Equals(""))
                //GameAnalytics.NewDesignEvent(AnalaticsEvent);
            EaseInCharacterDialog();
            _boardOffset = boardOffset;
            BoundsInt cellBounds = _tileMap.cellBounds;
            Vector2Int _tutorialBoardOffset = new Vector2Int(cellBounds.xMin, cellBounds.yMin);
            Vector2Int boardDiff = _tutorialBoardOffset - _boardOffset;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int worldPos = new Vector3Int(_boardOffset.x + x, _boardOffset.y + y, 0);
                    var tile = _tileMap.GetTile(worldPos);
                    if (tile != null)
                    {
                        string cellSpriteName = tile.name;
                        // Debug.Log($"Cheking worldpos: {worldPos}");
                        if (cellSpriteName.Contains("left"))
                        {
                            ValidMove validMove = new ValidMove() { Position = new Point(x, y), Direction = Direction.Left };
                            validMoves.Add(validMove);
                            // Debug.Log($"Added a valid move {validMove.Position} -> {validMove.Direction}");
                            _tileMap.SetTile(worldPos, null);
                        }
                        else if (cellSpriteName.Contains("right"))
                        {
                            ValidMove validMove = new ValidMove() { Position = new Point(x, y), Direction = Direction.Right };
                            validMoves.Add(validMove);
                            // Debug.Log($"Added a valid move {validMove.Position} -> {validMove.Direction}");
                            _tileMap.SetTile(worldPos, null);
                        }
                        else if (cellSpriteName.Contains("up"))
                        {
                            ValidMove validMove = new ValidMove() { Position = new Point(x, y), Direction = Direction.Top };
                            validMoves.Add(validMove);
                            // Debug.Log($"Added a valid move {validMove.Position} -> {validMove.Direction}");
                            _tileMap.SetTile(worldPos, null);
                        }
                        else if (cellSpriteName.Contains("down"))
                        {
                            ValidMove validMove = new ValidMove() { Position = new Point(x, y), Direction = Direction.Bottom };
                            validMoves.Add(validMove);
                            // Debug.Log($"Added a valid move {validMove.Position} -> {validMove.Direction}");
                            _tileMap.SetTile(worldPos, null);
                        }
                    }
                }
            }
        }

        private void EaseInCharacterDialog()
        {
            Transform dialogPanel = FindDeepChild(transform, "dialog-frame");
            if (dialogPanel != null)
            {
                var okButton = FindDeepChild(transform, "ok-button");
                if (okButton != null)
                {
                    okButton.SetParent(dialogPanel);
                }
                // Debug.Log("Dialog panel found!");
                var startScale = dialogPanel.transform.localScale;
                dialogPanel.transform.localScale = Vector3.zero;
                // var scaleUpTween = dialogPanel.DOScale(startScale, CharacterDialogBounceDurarion);
                // scaleUpTween.SetEase(Ease.OutBack, CharacterDialogBouncePower);
            }
        }

        Transform FindDeepChild(Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }

        internal bool IsValidSwap(Point pos, Direction direction)
        {
            foreach (var validSwap in validMoves)
            {
                if (validSwap.Position == pos && validSwap.Direction == direction)
                {
                    return true;
                }
            }
            return false;
        }
    }
}