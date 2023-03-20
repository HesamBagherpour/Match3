using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace HB.Match3.View
{
    public class BucketSpawnEffect : Effect
    {
        [SerializeField] SpriteRenderer collectSprite;
        [SerializeField] SpriteRenderer shadowSprite;
        [SerializeField] AnimationCurve bounceCurve;
        private Vector3 scaleUpValue;
        protected override void Awake()
        {
            if (collectSprite == null) collectSprite = GetComponent<SpriteRenderer>();
            if (shadowSprite == null) shadowSprite = GetComponentInChildren<SpriteRenderer>();
            scaleUpValue = new Vector3(2, 2, 2);
        }

        public override void Play(Vector3 destination)
        {
            float scaleDuration = clearDuration / 2;
            float moveDuration = clearDuration;
            const Ease outQuart = Ease.OutQuart;

            var moveTween = transform.DOMove(destination, moveDuration);
            moveTween.SetEase(outQuart);

            var rotateTween = transform.DORotate(new Vector3(0, 0, 360), moveDuration, RotateMode.FastBeyond360);

            var scaleUpTween = transform.DOScale(scaleUpValue, scaleDuration);
            scaleUpTween.SetEase(outQuart);

            scaleUpTween.onComplete += () =>
            {
                var scaleBack = transform.DOScale(Vector3.one, scaleDuration);
                // StartCoroutine(ShakeCamera(scaleDuration / 2));
                scaleBack.SetEase(outQuart);
                scaleBack.onComplete += () =>
                {
                    Complete();
                };
            };
        }

        private IEnumerator ShakeCamera(float delay)
        {
            yield return new WaitForSeconds(delay);
            CameraShake.DoShake();
        }

        public override void SetSprite(Sprite sprite)
        {
            collectSprite.sprite = sprite;
            shadowSprite.sprite = sprite;
        }
    }
}