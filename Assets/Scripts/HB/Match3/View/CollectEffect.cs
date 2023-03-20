using DG.Tweening;
using UnityEngine;

namespace HB.Match3.View
{
    public class CollectEffect : Effect
    {
        [SerializeField] SpriteRenderer collectSprite;
        [SerializeField] SpriteRenderer shadowSprite;
        private Vector3 scaleUpValue;

        protected override void Awake()
        {
            if (collectSprite == null) collectSprite = GetComponent<SpriteRenderer>();
            if (shadowSprite == null) shadowSprite = GetComponentInChildren<SpriteRenderer>();
            scaleUpValue = new Vector3(2, 2, 2);
        }

        public override void Play(Vector3 destination)
        {
            var moveTween = transform.DOMove(destination, clearDuration);
            moveTween.SetEase(Ease.InOutQuart);
            moveTween.onComplete += OnMoveComplete;
            float duration = clearDuration / 2;
            var scaleTween = transform.DOScale(scaleUpValue, duration);
            const Ease outQuart = Ease.OutQuart;
            scaleTween.SetEase(outQuart);
            scaleTween.onComplete += () =>
            {
                var scaleBack = transform.DOScale(Vector3.one, duration);
                scaleBack.SetEase(Ease.InQuart);
            };
        }

        public override void SetSprite(Sprite sprite)
        {
            collectSprite.sprite = sprite;
            shadowSprite.sprite = sprite;
        }

        private void OnMoveComplete()
        {
            Complete();
        }
    }
}