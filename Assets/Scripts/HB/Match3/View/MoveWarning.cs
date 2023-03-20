using DG.Tweening;
using Garage.Match3.View;
using TMPro;
using UnityEngine;

namespace HB.Match3.View
{
    public class MoveWarning : Effect
    {
        [SerializeField] TextMeshPro textComponent;
        private Color startColor;

        public override void Play()
        {
            Debug.Log("Playing Warning effect");

            startColor = textComponent.color;
            var tmpColor = textComponent.color;
            tmpColor.a = 0;
            textComponent.color = tmpColor;

            var fadeInTween = textComponent.DOFade(1, clearDuration / 2);
            fadeInTween.SetEase(Ease.OutQuart);

            transform.localScale = Vector3.zero;
            var scaleUpTween = transform.DOScale(Vector3.one, clearDuration / 2);
            scaleUpTween.SetEase(Ease.OutElastic, 2);
            scaleUpTween.SetRelative(true);
            scaleUpTween.onComplete += () =>
            {
                textComponent.color = startColor;
                var fadeOutTween = textComponent.DOFade(0, clearDuration / 2);
                fadeOutTween.SetEase(Ease.Linear);

                var scaleDownTween = transform.DOScale(Vector3.zero, clearDuration / 2);
                scaleDownTween.SetEase(Ease.InElastic);
                scaleDownTween.onComplete += () =>
                {
                    textComponent.color = startColor;
                    OnMoveComplete();
                };
            };


        }
        protected override void Update() { }
        private void OnMoveComplete()
        {
            Complete();
        }

        private void OnDisable()
        {
            transform.localScale = Vector3.zero;
        }

    }
}