using System;
using System.Collections.Generic;
using HB.Match3.Block;
using HB.Match3.Cell;
using HB.Match3.Cell.Effect;
using HB.Match3.Models;
using HB.Match3.View;
using UnityEngine;


namespace HB.Match3.Modules
{
    public class BoosterModuleView : ObjectModuleView
    {
        private BoosterType _boosterType;
        private BoosterView _boosterView;
        private List<Effect> _effects;
        private Action _onFinish;
        private BlockType _blockType;
        private int _effectCounter;
        private Effect starAppearEffect;
        private Effect starTrailEffect;
          public BoosterModuleView(BaseModule module) : base(module)
        {
        }

        public override void SetGameObject(GameObject go)
        {
            _boosterView = go.GetComponent<BoosterView>();
        }

        public void Hit(string trailId, Point currentPos, List<Point> targetPositions, Action onFinish)
        {
            _effectCounter = 0;
            _onFinish = onFinish;
            _effects = new List<Effect>(targetPositions?.Count ?? 1);
            switch (_boosterType)
            {
                case BoosterType.Horizontal:
                case BoosterType.Vertical:
                case BoosterType.Cross:
                    Effect boosterDefaultEffect = Layer.PlayEffect(_boosterView.transform.position, _boosterType.ToString().ToLower());
                    AddToEffects(boosterDefaultEffect);
                    break;
                case BoosterType.Square:
                    PlayStarAppearEffect(trailId, currentPos, targetPositions);
                    break;
                case BoosterType.Rainbow:
                    PlayTrailEffect(trailId, currentPos, targetPositions);
                    break;
                case BoosterType.Explosion:
                    string id = _blockType.ToString().ToLower() + "-jumbo";
                    Effect chaghBlockExplotion = Layer.PlayEffect(_boosterView.transform.position, id);
                    AddToEffects(chaghBlockExplotion);
                    break;
                case BoosterType.None:
                default:
                    break;
            }

            if (_effects.Count == 0)
            {
                _onFinish?.Invoke();
                _onFinish = null;
            }

            Layer.Clear(this);
        }

        private void PlayStarAppearEffect(string trailId, Point currentPos, List<Point> targetPositions)
        {
            Vector3 currentWorldPosition = Layer.CellToWorld(currentPos);
            starAppearEffect = Layer.PlayEffect(currentWorldPosition, "star-start");
            starAppearEffect.OnClear += StarAppearCleared(trailId, currentPos, targetPositions);
            AddToEffects(starAppearEffect);
        }

        private Action StarAppearCleared(string trailId, Point currentPos, List<Point> targetPositions)
        {
            return () =>
            {
                starAppearEffect.OnClear -= StarAppearCleared(trailId, currentPos, targetPositions);
                if (trailId != string.Empty && targetPositions != null && targetPositions.Count != 0)
                {
                    Vector3 currentWorldPosition = Layer.CellToWorld(currentPos);
                    for (int i = 0; i < targetPositions.Count; i++)
                    {
                        Point pos = targetPositions[i];
                        starTrailEffect = Layer.PlayEffect(currentWorldPosition, Layer.CellToWorld(pos), trailId);
                        starTrailEffect.OnClear += StarTrailCleared(pos);
                        AddToEffects(starTrailEffect);
                    }
                }
            };
        }

        private Action StarTrailCleared(Point pos)
        {
            return () =>
            {
                starTrailEffect.OnClear -= StarTrailCleared(pos);
                var starEndEffect = Layer.PlayEffect(Layer.CellToWorld(pos), "star-end");
                AddToEffects(starEndEffect);
            };
        }

        private void PlayTrailEffect(string trailId, Point currentPos, List<Point> targetPositions)
        {
            if (trailId != string.Empty && targetPositions != null && targetPositions.Count != 0)
            {
                Vector3 currentWorldPosition = Layer.CellToWorld(currentPos);
                for (int i = 0; i < targetPositions.Count; i++)
                {
                    Point pos = targetPositions[i];
                    Effect trailEffect = Layer.PlayEffect(currentWorldPosition, Layer.CellToWorld(pos), trailId);
                    AddToEffects(trailEffect);
                }
                if (_boosterType == BoosterType.Rainbow)
                {
                    if (_blockType != BlockType.None)
                    {
                        // Debug.Log("Playing rainbow");
                        Effect effect = Layer.PlayEffect(currentWorldPosition, $"rainbow-{_blockType.ToString()}");
                        AddToEffects(effect);
                    }
                }
            }
        }

        private void AddToEffects(Effect e)
        {
            if (e == null) return;
            _effectCounter++;
            e.OnClear += OnEffectClear;
            _effects.Add(e);
        }

        private void OnEffectClear()
        {
            _effectCounter--;
            if (_effectCounter == 0)
            {
                _onFinish?.Invoke();
                for (int i = 0; i < _effects.Count; i++)
                {
                    _effects[i].OnClear -= OnEffectClear;
                }
            }
        }

        public override void Clear(Action onFinished)
        {
            _boosterView.Clear();
            onFinished?.Invoke();
        }

        internal void SetBlockType(BlockType blockType)
        {
            _blockType = blockType;
        }

        public void SetType(BoosterType boosterType)
        {
            _boosterType = boosterType;
            _boosterView.SetType(boosterType);
        }
    }
    
}