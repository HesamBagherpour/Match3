using System;
using System.Collections.Generic;
using HB.Match3.Block;
using UnityEngine;

namespace HB.Match3.Cell.Effect
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