using UnityEngine;
using System;

namespace HB.Packages.GUI.Effect
{
    public class ParticleWrapper : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        //private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            // print(_particleSystem.main.duration);
        }
        public void Play(Action onFinished)
        {
            _particleSystem.Play();
            // Observable.EveryUpdate()
            //     .Where(_ => (_particleSystem.main.duration - _particleSystem.time) <= 0.1f).Subscribe(
            //         _ =>
            //         {
            //             _particleSystem.Stop();
            //             onFinished?.Invoke();
            //             _disposables.Clear();
            //         }
            //     ).AddTo(_disposables);
        }

    }
}