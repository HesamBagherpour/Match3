using System.Collections.Generic;
using Garage.Match3;
using UnityEngine;

namespace HB.Match3.View
{
    public class BucketBurstEffect : Effect
    {
        [SerializeField] List<Color> allColors;
        [SerializeField] List<BlockColor> allBlockColors;

        ParticleSystem[] allParticles;

        protected override void Awake()
        {
            base.Awake();
            allParticles = GetComponentsInChildren<ParticleSystem>();
        }

        public void SetColor(BlockColor blockColor)
        {
            int index = allBlockColors.FindIndex(x => x == blockColor);
            foreach (var ps in allParticles)
            {
                var mainModule = ps.main;
                mainModule.startColor = allColors[index];
            }
        }
    }
}