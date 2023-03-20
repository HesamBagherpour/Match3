using System.Collections.Generic;
using Garage.Match3.Cells.Modules;
using Lean.Pool;
using UnityEngine;

namespace Garage.Match3.View
{
    public class BoosterView : MonoBehaviour
    {
        [SerializeField] private List<BoosterType> types;
        [SerializeField] private List<GameObject> boosterPrefabs;
        private GameObject boosterGo;

        public void SetType(BoosterType type)
        {
            int index = types.FindIndex(boosterType => boosterType == type);
            DeleteAllChildren();

            if (index != -1)
            {
                boosterGo = LeanPool.Spawn(boosterPrefabs[index], transform);
                var booster = boosterGo.GetComponent<ParticleSystem>();
                booster.transform.localPosition = Vector3.zero;
                booster.Play();
            }
        }

        private void DeleteAllChildren()
        {
            if (boosterGo != null)
            {
                LeanPool.Despawn(boosterGo);
                boosterGo = null;
            }
        }

        public void Clear()
        {
            DeleteAllChildren();
            gameObject.SetActive(false);
        }
    }
}