using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HB.Match3.View
{
    public class BoardEffects : MonoBehaviour
    {
        [SerializeField] private List<ParticlePlair> explotionPrefabs = new List<ParticlePlair>();
        private List<ParticlePlair> _particlePool = new List<ParticlePlair>();
        private Transform _parent;

        public void Initialize(int warmUpCount, Transform parent)
        {
            this._parent = parent;
            for (int i = 0; i < explotionPrefabs.Count; i++)
            {
                for (int j = 0; j < warmUpCount; j++)
                {
                    ParticlePlair pair = explotionPrefabs[i];
                    var particleObject = GameObject.Instantiate(pair.particleSystem, parent, false);
                    particleObject.gameObject.SetActive(false);
                    var pr = new ParticlePlair()
                    {
                        name = pair.name,
                        particleSystem = particleObject
                    };
                    _particlePool.Add(pr);
                }
            }
        }
        public async UniTask PlayEffect(Vector3 worldPos, string effectName, float delay)
        {
            var psIndex = _particlePool.FindIndex(x => x.name == effectName && x.particleSystem.gameObject.activeInHierarchy == false);
            if (psIndex != -1)
            {
                ParticleSystem particle = _particlePool[psIndex].particleSystem;
                particle.transform.position = worldPos;
                particle.gameObject.SetActive(true);
                particle.Play();
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
                DeactiveParticle(particle);
            }
            else
            {
                // instantiate a new one and add it to pool
                var particlePrefabIndex = explotionPrefabs.FindIndex(x => x.name == effectName);
                if (particlePrefabIndex != -1)
                {
                    ParticleSystem particlePrefab = explotionPrefabs[particlePrefabIndex].particleSystem;
                    var particleObject = GameObject.Instantiate(particlePrefab, worldPos, Quaternion.identity);
                    particleObject.transform.SetParent(_parent, false);
                    var pr = new ParticlePlair()
                    {
                        name = effectName,
                        particleSystem = particleObject
                    };
                    _particlePool.Add(pr);
                    particleObject.Play();
                    await UniTask.Delay(TimeSpan.FromSeconds(delay));
                    DeactiveParticle(particleObject);
                }
            }
        }

        private async void DeactiveParticle(ParticleSystem particleSystem)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(particleSystem.main.duration));
            particleSystem.Stop();
            particleSystem.gameObject.SetActive(false);
        }

    }
}