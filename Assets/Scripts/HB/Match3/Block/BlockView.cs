using System;
using HB.Match3.Cell;
using HB.Packages.Utilities;
using TMPro;
using UnityEngine;

namespace HB.Match3.Block
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
        
        
        #region Private Fields

        [SerializeField] TextMeshPro textComponent;
        public Transform contentTransform;
        public SpriteRenderer blockSprite;
        public SpriteRenderer couponSprite;
        private Vector2 _moveDirection;
        private Vector2 _startPress;
        private Vector2 _endRelease;
        private ClickableObject _clickable;
        //private BlockAnimator _blockAnimator;
        private int _count;
        #endregion
    }
}