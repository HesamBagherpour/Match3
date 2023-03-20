using System.Collections.Generic;
using Garage.Match3;
using Garage.Match3.View;
using UnityEngine;

namespace HB.Match3.View
{
    public class BucketView : MonoBehaviour
    {
        [SerializeField] private List<BucketBehaviour> allBuckets;
        BucketBehaviour currentBucket = null;
        internal void SetHealth(BlockColor color, int health)
        {
            var bucketIndex = allBuckets.FindIndex(x => x.color == color && x.health == health);
            DeleteAllChildren();
            InstantiateNewChild(bucketIndex);
        }

        private void InstantiateNewChild(int bucketIndex)
        {
            currentBucket = Instantiate(allBuckets[bucketIndex], transform).GetComponent<BucketBehaviour>();
            currentBucket.transform.localPosition = Vector3.zero;
        }

        private void DeleteAllChildren()
        {
            if (currentBucket != null)
                Destroy(currentBucket.gameObject);
        }
    }
}