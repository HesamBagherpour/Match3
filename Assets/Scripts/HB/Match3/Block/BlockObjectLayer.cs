using System.Collections.Generic;
using HB.Match3.Modules;
using HB.Match3.View;
using UnityEngine;
using Object = UnityEngine.Object;



namespace HB.Match3.Block
{
    public class BlockObjectLayer : ObjectLayer
    {
        private GameObject _sweepEffect;
        private const string SweepPrefabName = "VFXSweep";

        
        public static class ListShuffleExtiontion
        {
            private static System.Random rng = new System.Random();
            public static void Shuffle<T>(IList<T> list)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
            }
        }
        
        
        public BlockObjectLayer(int order, string name, TilemapSettings tilemapSettings, BoardViewData viewData,
            LayerStack layerStack) :
            base(
                order,
                name,
                tilemapSettings,
                viewData,
                layerStack)
        {
            GameObject sweepEffectPrefab = viewData.layerViewData.allPrefabs.Find(go => go.name == SweepPrefabName);
            _sweepEffect = Object.Instantiate(sweepEffectPrefab);
            _sweepEffect.gameObject.SetActive(false);
        }

        public void Sweep(Vector3 pos, Vector3 targetPosition)
        {
            if (_sweepEffect != null)
            {
                _sweepEffect.SetActive(true);
                _sweepEffect.transform.position = pos;
                _sweepEffect.transform.LookAt(targetPosition);
                ParticleSystem ps = _sweepEffect.GetComponent<ParticleSystem>();
                ps.Play();
            }
        }
    }
    
}