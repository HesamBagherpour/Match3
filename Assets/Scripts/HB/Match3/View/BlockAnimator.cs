using System;
using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Garage.Match3.BoardStates;
using Garage.Match3.View;
using UnityEngine;

namespace HB.Match3.View
{
    public class BlockAnimator : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        private const float moveSuggestDistance = 0.23f;
        private const float SuggestSingleLoopDuration = 0.1f;

        #region  Static
        private static readonly Vector3 JumboScale = new Vector3(1.4f, 1.4f, 1.4f);
        private static readonly Vector3 HorizontalScale = new Vector3(1f, 0.9f, 1f);
        private static readonly Vector3 VerticalScale = new Vector3(0.9f, 1f, 1f);
        private static readonly Vector3 SuggestScale = new Vector3(0.1f, 0.1f, 0.1f);
        private static readonly Vector2 _selectScale = new Vector2(1.1f, 1.1f);
        #endregion

        #region Private Fields
        private Vector3 _initScale;
        private bool isJumbo;
        private Vector3 beforeSuggestPosition;
        private Vector3 beforeSuggestScale;
        private int moveSuggestTweenId;
        private int scaleSuggestTweenID;

        #region BounceFields
        private bool bounceStart;
        private Vector3 bounceStartPos;
        private int bounceFrameCounter;
        private const int BounceWaitFrames = BlockView.Setting.BounceWaitFrames;
        private TweenerCore<Vector3, Vector3, VectorOptions> bounceTween;
        #endregion

        public Vector3 NormalScale => isJumbo ? JumboScale : _initScale;

        #endregion

        #region Unity
        private void Awake()
        {
            _initScale = transform.localScale;
        }

        private void OnEnable()
        {
            transform.localScale = _initScale;
            SwapState.DiscardSuggest += DiscardSuggest;
            moveSuggestTweenId = -1;
            scaleSuggestTweenID = -1;
            isJumbo = false;
        }
        private void OnDisable()
        {
            SwapState.DiscardSuggest -= DiscardSuggest;
        }

        private void Update()
        {
            if (bounceStart)
            {
                if (bounceFrameCounter < BounceWaitFrames)
                {
                    bounceFrameCounter++;
                }
                else
                {
                    PlayBounce();
                    bounceStart = false;
                    bounceFrameCounter = 0;
                }
            }
        }

        #endregion

        #region Public Methods
        public void PlayJumbo()
        {
            isJumbo = true;
            var scaleTween = transform.DOScale(JumboScale, 0.7f);
            scaleTween.SetEase(Ease.OutElastic);
        }

        public void PlayMove(Direction direction)
        {
            PlayIdle();
            switch (direction)
            {
                case Direction.None:
                case Direction.Center:
                case Direction.All:
                    break;
                case Direction.Top:
                case Direction.Bottom:
                    transform.localScale = Vector3.Scale(transform.localScale, VerticalScale);
                    break;
                case Direction.Left:
                case Direction.Right:
                    transform.localScale = Vector3.Scale(transform.localScale, HorizontalScale);
                    break;
                default:
                    break;
            }
        }

        public void PlayIdle()
        {
            transform.localScale = NormalScale;
        }

        internal void PlayClear(Action onClearFinished)
        {
            var tween = transform.DOScale(BlockView.Setting.ClearScaleUp, 0.2f);
            tween.onComplete += () =>
            {
                onClearFinished?.Invoke();
            };
            tween.Play();
        }

        internal void ReadyToBounce()
        {
            bounceStartPos = transform.position;
            bounceStart = true;
            bounceFrameCounter = 0;
        }

        private void PlayBounce()
        {
            _animator.SetBool("Bounce", true);
            StartCoroutine(StopBounce());
            // _animator.SetBool("Bounce", false);
            // transform.position = bounceStartPos;
            // bounceTween = transform.DOMove(new Vector3(0, BlockView.Setting.BounceDistance, 0), BlockView.Setting.BounceDuration);
            // bounceTween.SetRelative(true);
            // bounceTween.SetEase(Ease.OutQuart);
            // bounceTween.SetLoops(2, LoopType.Yoyo);
        }

        private IEnumerator StopBounce()
        {
            yield return null;
            _animator.SetBool("Bounce", false);

        }

        public void ResetBounce()
        {
            bounceStart = false;
            bounceFrameCounter = 0;
        }


        internal void ResetSuggest(Vector3 position)
        {
            beforeSuggestPosition = position;
        }

        internal void SuggestMove(Direction direction, bool move)
        {
            beforeSuggestPosition = transform.position;
            beforeSuggestScale = transform.localScale;
            if (move)
            {
                PlayMove(direction);
                var moveSuggestTween = transform.DOMove(GetSuggestEndValue(direction), SuggestSingleLoopDuration);
                moveSuggestTween.SetEase(Ease.OutQuart);
                moveSuggestTween.SetLoops(4, LoopType.Yoyo);
                moveSuggestTween.SetRelative(true);
                moveSuggestTween.onComplete += () =>
                {
                    PlayIdle();
                    transform.position = beforeSuggestPosition;
                };
                moveSuggestTween.Play();
                moveSuggestTweenId = moveSuggestTween.intId;
            }

            var scaleSuggestTween = transform.DOScale(SuggestScale, SuggestSingleLoopDuration);
            scaleSuggestTween.SetEase(Ease.OutQuart);
            scaleSuggestTween.SetLoops(4, LoopType.Yoyo);
            scaleSuggestTween.SetRelative(true);
            scaleSuggestTween.onComplete += () =>
            {
                PlayIdle();
                transform.localScale = beforeSuggestScale;
            };
            scaleSuggestTween.Play();
            scaleSuggestTweenID = scaleSuggestTween.intId;
        }

        private Vector3 GetSuggestEndValue(Direction direction)
        {
            Vector3 endValue = Vector3.zero;
            switch (direction)
            {
                case Direction.None:
                case Direction.Center:
                case Direction.All:
                    return endValue;
                case Direction.Top:
                    endValue = new Vector3(0, moveSuggestDistance, 0);
                    break;
                case Direction.Left:
                    endValue = new Vector3(-moveSuggestDistance, 0, 0);
                    break;
                case Direction.Bottom:
                    endValue = new Vector3(0, -moveSuggestDistance, 0);
                    break;
                case Direction.Right:
                    endValue = new Vector3(moveSuggestDistance, 0, 0);
                    break;
                default:
                    return endValue;
            }
            return endValue;
        }

        private void DiscardSuggest()
        {
            if (moveSuggestTweenId != -1)
            {
                DOTween.Complete(moveSuggestTweenId);
                transform.position = beforeSuggestPosition;
                moveSuggestTweenId = -1;
            }
            if (scaleSuggestTweenID != -1)
            {
                DOTween.Complete(scaleSuggestTweenID);
                transform.localScale = beforeSuggestScale;
                scaleSuggestTweenID = -1;
            }
        }

        public void PlaySelect()
        {
            transform.DOScale(Vector3.Scale(NormalScale, _selectScale), 0.1f);
        }

        public void PlayRelease()
        {
            PlayIdle();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}