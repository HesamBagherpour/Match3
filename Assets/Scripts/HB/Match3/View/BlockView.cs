using System;
using DG.Tweening;
using UnityEngine;
using HB.Match3;
using HB.Match3.View;
using HB.Utilities;
using TMPro;

namespace Garage.Match3.View
{
    public class BlockView : MonoBehaviour
    {
        public static class Setting
        {
            private const float Speed = 1.5f;
            public const float MoveDuration = 0.2f / Speed;
            public const float ReshuffleDuration = 2.5f / Speed;
            public const float FallDuration = 0.12f / Speed;
            public const float BounceDistance = 0.23f;
            public static Vector3 ClearScaleUp = new Vector3(1.2f, 1.2f, 1.2f);
            public const int BounceWaitFrames = 1;
            public const float BounceDuration = FallDuration - 0.01f;

        }

        public static class ColorSetting
        {
            public static Color brightCyan = ColorFromHex("#34ebdc");
            public static Color brightRed = ColorFromHex("#eb3434");
            public static Color brightOrange = ColorFromHex("#eb8334");
            public static Color brightYellow = ColorFromHex("#ebe834");
            public static Color brightPurple = ColorFromHex("#eb34c9");
            public static Color brightGreen = ColorFromHex("#4feb34");
            public static Color ColorFromHex(string hexColor)
            {
                Color color = Color.white;
                ColorUtility.TryParseHtmlString(hexColor, out color);
                return color;
            }
        }

        #region Events
        public event Action<Direction> SwapRequest;
        public event Action PressRequest;
        #endregion

        #region  Static

        #endregion

        #region Private Fields

        [SerializeField] TextMeshPro textComponent;
        public Transform contentTransform;
        public SpriteRenderer blockSprite;
        public SpriteRenderer couponSprite;
        private Vector2 _moveDirection;
        private Vector2 _startPress;
        private Vector2 _endRelease;
        private ClickableObject _clickable;
        private BlockAnimator _blockAnimator;
        private int _count;
        #endregion

        #region Public Properties

        #endregion

        #region IBlockView Interface


        internal void Reshuffle(Vector3 position, Action onMoveFinished)
        {
            var tween = transform.DOMove(position, Setting.ReshuffleDuration);
            tween.SetEase(Ease.InOutElastic, 0.01f);
            tween.OnComplete(() =>
            {
                _blockAnimator.PlayIdle();
                onMoveFinished?.Invoke();
            });
        }

        public void MoveTo(Vector3 position, Action onMoveFinished)
        {
            Direction direction = Utils.GetDirection(position - transform.position);
            _blockAnimator.PlayMove(direction);
            var tween = transform.DOMove(position, Setting.MoveDuration);
            tween.SetEase(Ease.OutQuart);
            tween.OnComplete(() =>
            {
                _blockAnimator.PlayIdle();
                onMoveFinished?.Invoke();
            });
        }

        public void Fall(Vector3 position, Action onFallFinished)
        {
            _blockAnimator.ResetBounce();
            _blockAnimator.ResetSuggest(position);
            Direction direction = Utils.GetDirection(position - transform.position);
            _blockAnimator.PlayMove(direction);
            var fallTween = transform.DOMove(position, Setting.FallDuration);
            fallTween.SetEase(Ease.Linear);
            fallTween.OnComplete(() =>
            {
                _blockAnimator.ReadyToBounce();
                _blockAnimator.PlayIdle();
                onFallFinished?.Invoke();
            });
        }

        public void SetCoupon(bool state)
        {
            couponSprite?.gameObject.SetActive(state);
        }

        public void Clear(Action onClearFinished)
        {
            _blockAnimator.PlayClear(() =>
            {
                onClearFinished?.Invoke();
            });
        }

        internal void SuggestMove(Direction direction, bool move)
        {
            _blockAnimator.SuggestMove(direction, move);
        }
        //protected readonly FastStringBuilder finalText = new FastStringBuilder(RTLSupport.DefaultBufferSize);
        public void SetCount(int count, BlockColor color)
        {
            _count = count;
            if (_count <= 1)
            {
                textComponent?.gameObject.SetActive(false);
                textComponent.color = Color.white;
            }
            else
            {
                if (textComponent.gameObject.activeInHierarchy == false) textComponent.gameObject.SetActive(true);
                Color _textColor = Color.white;
                switch (color)
                {
                    case BlockColor.Red:
                        _textColor = ColorSetting.brightRed;
                        break;
                    case BlockColor.Green:
                        _textColor = ColorSetting.brightGreen;
                        break;
                    case BlockColor.Cyan:
                        _textColor = ColorSetting.brightCyan;
                        break;
                    case BlockColor.Purple:
                        _textColor = ColorSetting.brightPurple;
                        break;
                    case BlockColor.Yellow:
                        _textColor = ColorSetting.brightYellow;
                        break;
                    case BlockColor.Orange:
                        _textColor = ColorSetting.brightOrange;
                        break;
                    case BlockColor.None:
                        _textColor = Color.white;
                        break;
                    default:
                        break;
                }
                textComponent.color = _textColor;
                var shakeTween = textComponent.transform.DOShakeScale(0.5f, 0.5f);
                shakeTween.onComplete += () => { textComponent.transform.localScale = Vector3.one; };


                textComponent.text = "ddddddddddddddddddd";
            }
        }

        #endregion

        #region Unity

        private void Awake()
        {
            _clickable = GetComponent<ClickableObject>();
            _blockAnimator = GetComponent<BlockAnimator>();
        }

        private void OnDisable()
        {
            _clickable.Pressed -= OnPressed;
            _clickable.Released -= OnReleased;
            DOTween.Kill(this, false);
        }

        private void OnEnable()
        {
            _clickable.Pressed += OnPressed;
            _clickable.Released += OnReleased;
            transform.localScale = Vector3.one;
            _blockAnimator.PlayIdle();
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return $"{name}";
        }

        #endregion

        #region Private Methods

        private void OnPressed()
        {
            PressRequest?.Invoke();
            _startPress = Input.mousePosition;
            _blockAnimator.PlaySelect();
        }

        private void OnReleased()
        {
            _blockAnimator.PlayRelease();
            _endRelease = Input.mousePosition;
            var moveDirection = _endRelease - _startPress;
            Direction dir = Utils.GetDirection(moveDirection);
            if (dir != Direction.Center && dir != Direction.None) SwapRequest?.Invoke(dir);
        }

        public void SetJumbo()
        {
            if (_blockAnimator == null) _blockAnimator = GetComponent<BlockAnimator>();
            _blockAnimator.PlayJumbo();
        }

        #endregion
    }
}