using System;
using System.Collections.Generic;
using DG.Tweening;
using Garage.HomeDesign.Ui_Menus;
using Garage.Match3;
using Garage.Match3.Cells;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Logger;
using HB.Match3.Cells.Modules;
using UnityEngine;

namespace HB.Match3.View
{
    public class BlockModuleView : ObjectModuleView
    {
        private BlockView _blockView;
        private BlockType _blockType;
        private SpriteRenderer _spriteRenderer;
        private static Dictionary<BlockType, Sprite> _blockTypeToSprite;
        private Effect jumboEffect;
        private bool hasCoupon;
        private static Sprite _couponSprite;
        private Effect plantAnimation;
        private bool _mergeIntoOther;

        public bool IgnoreQuestBySquareMatch { get; internal set; }

        public event Action<Direction> SwapRequest;
        public event Action PressRequest;

        public void MoveTo(Point point, Action onComplete, bool sweep)
        {
            Vector3 targetPosition = Layer.CellToWorld(point);
            if (sweep)
            {
                BlockObjectLayer blockObjectLayer = (BlockObjectLayer)Layer;
                blockObjectLayer.Sweep(_blockView.transform.position, targetPosition);
            }
            _blockView.MoveTo(targetPosition, onComplete);
        }

        internal void Reshuffle(Point point, Action onFinished)
        {
            Vector3 targetPosition = Layer.CellToWorld(point);
            _blockView.Reshuffle(targetPosition, onFinished);
        }

        public void Fall(Point point, Action onComplete)
        {
            Vector3 targetPosition = Layer.CellToWorld(point);
            _blockView.Fall(targetPosition, onComplete);
        }

        public override void SetGameObject(GameObject go)
        {
            _blockView = go.GetComponent<BlockView>();
            _blockView.SwapRequest += OnSwapRequest;
            _blockView.PressRequest += OnPressRequest;
            _spriteRenderer = _blockView.blockSprite;
            _couponSprite = _blockView.couponSprite.sprite;
            _spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            UpdateSprite();
        }


        private void UpdateSprite()
        {
            IgnoreQuestBySquareMatch = false;
            _mergeIntoOther = false;
            ReleasePlantEffect();
            _spriteRenderer.gameObject.SetActive(true);
            _spriteRenderer.enabled = true;
            if (_blockTypeToSprite.TryGetValue(_blockType, out Sprite sprite))
                _spriteRenderer.sprite = sprite;
            else
                Log.Error("BoardView", $"Could not found sprite for {_blockType.id}:{_blockType.color}");
            if (_blockType.id == BlockIDs.Plant)
            {
                _spriteRenderer.gameObject.SetActive(false);
                PlayPlantAnimation();
            }
        }

        internal void SetMergeIntoOther()
        {
            _mergeIntoOther = true;
        }

        private void PlayPlantAnimation()
        {
            if (Layer != null)
            {
                plantAnimation = Layer.PlayEffect(Vector3.zero, BlockIDs.Plant + "-animation");
                plantAnimation.transform.SetParent(_blockView.contentTransform, false);
                plantAnimation.transform.localPosition = Vector3.zero;
            }
        }

        internal void SuggestMove(Direction direction, bool move)
        {
            _blockView.SuggestMove(direction, move);
        }

        public void SetCount(int count, BlockColor color)
        {
            _blockView.SetCount(count, color);
        }

        public void SetCoupon(bool state)
        {
            hasCoupon = state;
            _blockView.SetCoupon(state);
        }

        internal void PlayMeterEffect(Action onFinished)
        {
            var effect = Layer.PlayEffect(_blockView.transform.position, "meter-move");
            effect.transform.SetParent(_blockView.transform, false);
            effect.transform.localPosition = Vector3.zero;
            var targetPos = new Vector3(0, 1, 0);
            effect.transform.DOMove(targetPos, 0.2f).SetRelative(true).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                onFinished?.Invoke();
                effect.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutQuart).OnComplete(() =>
                    {
                        Layer.ReleaseEffect(effect);
                    }
                );
            });
        }

        public override string ToString()
        {
            return $"{_blockType}";
        }

        private void OnSwapRequest(Direction dir)
        {
            SwapRequest?.Invoke(dir);
        }

        private void OnPressRequest()
        {
            PressRequest?.Invoke();
        }

        public BlockModuleView(BaseModule module, BlockViewDataBase blockViewData) : base(module)
        {
            _blockType = ((BlockModule)module).blockType;
            LayerName = module.layerName;
            if (_blockTypeToSprite == null) _blockTypeToSprite = blockViewData.ToDictionary();
        }

        public override void Clear(Action onFinished)
        {
            // play box audio
            if (_blockType.id == BlockIDs.Box) BoardView.PlayAudio("PaperBox");
            else if (jumboEffect != null && _mergeIntoOther == false)
                BoardView.PlayAudio("JumboBlockExplotion", 0.4f);
            else if (_blockType.id.Equals(BlockIDs.Nalbaki))
                BoardView.PlayAudio("nalbaki_explosion");

            string id = _blockType.ToString().ToLower();
            bool finishScheduled = false;
            if (_mergeIntoOther == false)
            {
                var clearEffect = Layer.PlayEffect(_blockView.transform.position, id);
                if (clearEffect != null)
                {
                    finishScheduled = true;
                    clearEffect.OnClear += () =>
                    {
                        BlockViewCleared();
                        Layer.Clear(this);
                        onFinished?.Invoke();
                    };
                }
            }
            // else if (jumboEffect != null && _mergingIntoOther == false)
            // {
            //     var clearEffect = Layer.PlayEffect(_blockView.transform.position, id + "-jumbo");
            //     if (clearEffect != null)
            //     {
            //         finishScheduled = true;
            //         clearEffect.OnClear += () =>
            //         {
            //             BlockViewCleared();
            //             Layer.Clear(this);
            //             onFinished?.Invoke();
            //         };
            //     }
            // }

            if (IgnoreQuestBySquareMatch == false && QuestGiver.IsInQuest(id))
            {
                var targetPos = Match3GameUi.QuestItemPosition(id);
                PlayCollectEffect(targetPos, _spriteRenderer.sprite);
            }

            if (_blockType.id == BlockIDs.Simple && IgnoreQuestBySquareMatch == false && QuestGiver.LockQuests.Count > 0)
            {
                foreach (var cell in QuestGiver.LockQuests)
                {
                    var lockModule = cell.GetModule<LockQuestModule>();
                    if (lockModule.color == _blockType.color)
                    {
                        var targetPos = Layer.CellToWorld(cell.position);
                        PlayCollectEffect(targetPos, _spriteRenderer.sprite, () => lockModule.PlayCollectEffect());
                    }
                }
            }

            if (hasCoupon)
            {
                var targetPos = Match3GameUi.QuestItemPosition("coupon");
                PlayCollectEffect(targetPos, _couponSprite);
            }
            SetCoupon(false);

            ReleaseJumboEffect();
            ReleasePlantEffect();

            if (finishScheduled == false)
            {
                _blockView.Clear(() =>
                {
                    BlockViewCleared();
                    Layer.Clear(this);
                    onFinished?.Invoke();
                });
            }
            else
            {
                BlockViewCleared();
            }
            IgnoreQuestBySquareMatch = false;
        }

        private void PlayCollectEffect(Vector2 targetPos, Sprite sprite, Action onComplete = null)
        {
            Effect collectEffect = Layer.PlayEffect(_blockView.transform.position, targetPos, "collect");
            collectEffect.SetSprite(sprite);
            collectEffect.OnComplete += () => { onComplete?.Invoke(); };
        }

        private void BlockViewCleared()
        {
            _blockView.SwapRequest -= OnSwapRequest;
            _blockView.PressRequest -= OnPressRequest;
            _spriteRenderer.enabled = false;
        }

        private void ReleasePlantEffect()
        {
            if (plantAnimation != null)
            {
                Layer.ReleaseEffect(plantAnimation);
                plantAnimation.gameObject.SetActive(false);
                plantAnimation.transform.SetParent(Layer.Grid, false);
                plantAnimation = null;
            }
        }

        private void ReleaseJumboEffect()
        {
            if (jumboEffect != null && Layer.ReleaseEffect(jumboEffect))
            {
                jumboEffect.gameObject.SetActive(false);
                jumboEffect.transform.SetParent(Layer.Grid, false);
                jumboEffect = null;
            }
        }

        public void SetBlockType(BlockType bt)
        {
            _blockType = bt;
            UpdateSprite();
        }

        internal void SetJumbo()
        {
            _blockView.SetJumbo();
            jumboEffect = Layer.PlayEffect(_blockView.transform.position, "jumbo");
            jumboEffect.transform.SetParent(_blockView.transform, true);
        }
        public override void Dispose()
        {
            base.Dispose();
            ReleaseJumboEffect();
            ReleasePlantEffect();
        }
    }
}