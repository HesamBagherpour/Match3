using System;
using System.Collections.Generic;
using DG.Tweening;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3.Cells.Modules;
using UnityEngine;

namespace HB.Match3.View
{
    public class CannonModuleView : ObjectModuleView
    {
        CannonView _cannonView;
        private Direction _direction;
        private List<Cell> cells;
        private Action _onOneCellPassed;
        private Effect cannonFireAnimation;
        private Effect cannonBallEffect;

        public CannonModuleView(BaseModule module) : base(module)
        {
            Visible = true;
            var cannonModule = (CannonModule)module;
            _direction = cannonModule.direction;
        }

        public override void Clear(Action onFinished)
        {
            onFinished?.Invoke();
        }

        public override void SetGameObject(GameObject go)
        {
            _cannonView = go.GetComponent<CannonView>();
            switch (_direction)
            {
                case Direction.Top:
                    _cannonView.transform.localRotation = Quaternion.Euler(0f, 0f, -90f);
                    break;
                case Direction.Left:
                    _cannonView.transform.localRotation = Quaternion.Euler(0f, 0f, 0);
                    break;
                case Direction.Right:
                    _cannonView.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                    break;
                case Direction.Bottom:
                    _cannonView.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                    break;
                case Direction.Center:
                case Direction.None:
                case Direction.All:
                    break;
                default:
                    break;
            }
        }

        internal void Fire(List<Cell> cells, Action onOneCellPassed)
        {
            this.cells = cells;
            _onOneCellPassed = onOneCellPassed;
            cannonFireAnimation = Layer.PlayEffect(_cannonView.transform.position, "cannon-fire-animation");
            // cannonFireAnimation.transform.SetParent(_cannonView.transform, false);
            cannonFireAnimation.transform.position = _cannonView.transform.position;
            cannonFireAnimation.transform.rotation = _cannonView.transform.rotation;
            cannonFireAnimation.OnClear += CannonFireCFinished;
        }

        private void CannonFireCFinished()
        {
            cannonFireAnimation.OnClear -= CannonFireCFinished;
            Layer.ReleaseEffect(cannonFireAnimation);
            cannonBallEffect = Layer.PlayEffect(_cannonView.transform.position, "cannon-ball");
            // cannonBallEffect.transform.SetParent(_cannonView.transform, false);
            cannonBallEffect.transform.position = _cannonView.transform.position;
            // cannonBallEffect.OnClear += CannonBallEffectFinished;
            const float moveDuration = 0.03f;
            for (int i = 0; i < cells.Count; i++)
            {
                Cell cell = cells[i];
                var cellPos = Layer.CellToWorld(cell.position);
                var moveTween = cannonBallEffect.transform.DOMove(cellPos, moveDuration);
                moveTween.SetEase(Ease.Linear);
                moveTween.SetDelay(moveDuration * i);
                moveTween.onComplete += () => _onOneCellPassed?.Invoke();
            }
        }

        internal void ShowFireEnd(Action onFireComplete)
        {
            var moveTween = cannonBallEffect.transform.DOMove(cannonBallEffect.transform.position, 0.2f);
            moveTween.onComplete += () => onFireComplete?.Invoke();
        }

        internal void FireComplete()
        {
            Layer.ReleaseEffect(cannonBallEffect);
        }
    }
}