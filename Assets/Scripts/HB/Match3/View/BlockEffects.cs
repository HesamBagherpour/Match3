using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HB.Match3.View
{
    [Serializable]
    public struct ParticlePlair
    {
        public string name;
        public ParticleSystem particleSystem;
    }
    public class BlockEffects : MonoBehaviour
    {
        [SerializeField] private List<ParticlePlair> explotions = new List<ParticlePlair>();
        public async UniTask PlayStarExplosion(BlockType type)
        {
            var psIndex = explotions.FindIndex(x => x.name == type.id);
            if (psIndex != -1)
            {
                ParticleSystem psPrefab = explotions[psIndex].particleSystem;
                var particle = GameObject.Instantiate(psPrefab, transform.position, Quaternion.identity);
                particle.Play();
                await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                DestroyParticle(particle);
            }
        }

        private async void DestroyParticle(ParticleSystem particleSystem)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(particleSystem.main.duration));
            particleSystem.Stop();
            Destroy(particleSystem.gameObject);
        }
    }
}