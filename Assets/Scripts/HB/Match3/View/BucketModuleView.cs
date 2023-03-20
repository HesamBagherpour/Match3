using System;
using Garage.Match3;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3.Cells.Modules;
using UnityEngine;

namespace HB.Match3.View
{
    public class BucketModuleView : ObjectModuleView
    {
        BucketView _bucketView;
        public BucketModuleView(BaseModule module) : base(module)
        {
            Visible = true;
            // var cannonModule = (BucketModule)module;
            // _direction = cannonModule.direction;
        }

        public override void Clear(Action onFinished)
        {
            onFinished?.Invoke();
        }

        public override void SetGameObject(GameObject go)
        {
            _bucketView = go.GetComponent<BucketView>();
        }
        internal void SetHealth(BlockColor color, int health, bool active = false)
        {
            _bucketView.SetHealth(color, health);
            if (active)
            {
                var impactEffect = (BucketBurstEffect)Layer.PlayEffect(_bucketView.transform.position, "bucket-active");
                impactEffect.SetColor(color);
            }
        }
    }
    public class FlowerModuleView : ObjectModuleView
    {
        FlowerView _flowerView;
        public FlowerModuleView(BaseModule module) : base(module)
        {
            Visible = true;
            // var cannonModule = (BucketModule)module;
            // _direction = cannonModule.direction;
        }

        public override void Clear(Action onFinished)
        {
            onFinished?.Invoke();
        }

        public override void SetGameObject(GameObject go)
        {
            _flowerView = go.GetComponent<FlowerView>();
        }
        internal void SetHealth(BlockColor color, int health, bool active = false)
        {
            _flowerView.SetHealth(color, health);
            if (active)
            {
                var impactEffect = (BucketBurstEffect)Layer.PlayEffect(_flowerView.transform.position, "bucket-active");
                impactEffect.SetColor(color);
            }
        }
    }
}