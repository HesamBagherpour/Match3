using System.Collections.Generic;
using UnityEngine;

namespace Garage.Match3.View
{
    public class FlowerView : MonoBehaviour
    {
        [SerializeField] private List<FlowerBehaviour> allBuckets;
        FlowerBehaviour currentBucket = null;
        internal void SetHealth(BlockColor color, int health)
        {
            var bucketIndex = allBuckets.FindIndex(x => x.color == color && x.health == health);
            DeleteAllChildren();
            InstantiateNewChild(bucketIndex);
        }

        private void InstantiateNewChild(int bucketIndex)
        {
            currentBucket = Instantiate(allBuckets[bucketIndex], transform).GetComponent<FlowerBehaviour>();
            currentBucket.transform.localPosition = Vector3.zero;
        }

        private void DeleteAllChildren()
        {
            if (currentBucket != null)
                Destroy(currentBucket.gameObject);
        }
    }
}