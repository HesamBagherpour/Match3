using System;
using DG.Tweening;
using Garage.Match3;
using UnityEngine;

namespace HB.Match3.View
{
    public class Effect : MonoBehaviour
    {
        public string id;
        private ParticleSystem _ps;
        public event Action OnComplete;
        public event Action OnClear;

        [SerializeField] protected float clearDuration = 1f;
        [SerializeField] protected bool deactiveOnComplete = true;
        [SerializeField] public int poolWarmUpCount = 5;
        public float ClearDuration => clearDuration;
        private float _clearElapsed;

        private float _completeDuration;
        private float _completeElapsed;

        protected virtual void Awake()
        {
            if (_ps == null)
            {
                _ps = GetComponent<ParticleSystem>();
                if (_ps != null) _completeDuration = _ps.main.duration;
            }
        }

        public virtual void Play()
        {
            _completeElapsed = 0;
            _clearElapsed = 0;
            if (_ps != null) _ps.Play();
        }

        public virtual void Play(Vector3 destination)
        {
            Play();
            transform.DOMove(destination, clearDuration);
        }

        public virtual void SetSprite(Sprite sprite) { }
        public virtual void SetCountAndColor(int count, BlockColor color) { }

        protected virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            if (_clearElapsed >= clearDuration)
            {
                if (OnClear != null)
                {
                    OnClear.Invoke();
                    OnClear = null;
                }

                _clearElapsed = 0;
            }
            else
            {
                _clearElapsed += deltaTime;
            }

            if (deactiveOnComplete)
            {
                if (_completeElapsed >= _completeDuration)
                {
                    Complete();
                    _completeElapsed = 0;
                }
                else
                {
                    _completeElapsed += deltaTime;
                }
            }
        }

        protected void Complete()
        {
            OnComplete?.Invoke();
            OnComplete = null;
        }

        private void OnDisable()
        {
            DOTween.Kill(this, false);
            if (_ps != null) _ps.Stop();
            RemoveAllListeners();
        }

        internal void RemoveAllListeners()
        {
            OnComplete = null;
            OnClear = null;
        }
    }
}