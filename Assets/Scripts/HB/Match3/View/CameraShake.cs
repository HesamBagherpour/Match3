using DG.Tweening;
using UnityEngine;

namespace HB.Match3.View
{
    public class CameraShake : MonoBehaviour
    {
        public float duration = 0.3f;
        public float strenght = 0.4f;
        public int vibrato = 1000;
        public int randomness = 20;
        private static bool isShaking;
        private static Vector3 startPos;
        private static CameraShake instance;

        private void Awake()
        {
            if (instance == null) instance = this;
            startPos = transform.position;
        }

        public static void DoShake()
        {
            if (instance == null) return;
            if (isShaking) return;
            isShaking = true;
            var shakeTween = instance.transform.DOShakePosition(instance.duration, instance.strenght, instance.vibrato, instance.randomness, false, true);
            shakeTween.onComplete += () =>
            {
                instance.transform.position = startPos;
                isShaking = false;
            };
        }
    }
}