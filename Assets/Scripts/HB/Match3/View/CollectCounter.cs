using DG.Tweening;
using Garage.Match3;
using TMPro;
using UnityEngine;

namespace HB.Match3.View
{
    public class CollectCounter : Effect
    {
        private const float scaleUpFactor = 1.5f;
        private Vector3 scaleUpValue;
        [SerializeField] TextMeshPro textComponent;
        protected override void Awake()
        {
           //if (textComponent == null) textComponent = GetComponentInChildren<RTLTextMeshPro>();
            scaleUpValue = new Vector3(scaleUpFactor, scaleUpFactor, scaleUpFactor);
        }

        public override void Play()
        {
            // Debug.Log("Playing effect collect counter");
            Vector3 destination = new Vector3(0, 1, 0);
            var moveTween = transform.DOMove(destination, clearDuration);
            moveTween.SetEase(Ease.InQuart);
            moveTween.onComplete += OnMoveComplete;
            moveTween.SetRelative(true);
            float duration = clearDuration / 3;
            var scaleTween = transform.DOScale(scaleUpValue, duration);
            const Ease outQuart = Ease.OutQuart;
            scaleTween.SetEase(outQuart);
        }
        protected override void Update() { }
        //protected readonly FastStringBuilder finalText = new FastStringBuilder(RTLSupport.DefaultBufferSize);

        public override void SetCountAndColor(int count, BlockColor color)
        {
        
            // finalText.Clear();
            // RTLSupport.FixRTL("+" + count, finalText, true, true, true);
            // finalText.Reverse();
            textComponent.text = "dsadasdsadasdsada";
            Color textColor = Color.white;
            switch (color)
            {
                case BlockColor.None:
                    textColor = Color.gray;
                    break;
                case BlockColor.Red:
                    textColor = Color.red;
                    break;
                case BlockColor.Green:
                    textColor = Color.green;
                    break;
                case BlockColor.Cyan:
                    textColor = Color.cyan;
                    break;
                case BlockColor.Purple:
                    textColor = Color.magenta;
                    break;
                case BlockColor.Yellow:
                    textColor = Color.yellow;
                    break;
                case BlockColor.Orange:
                    textColor = new Color(1, 0.5f, 0.15f);
                    break;
                default:
                    break;
            }
            textComponent.color = textColor;
        }

        private void OnMoveComplete()
        {
            Complete();
            transform.localScale = Vector3.one;
        }
    }
}