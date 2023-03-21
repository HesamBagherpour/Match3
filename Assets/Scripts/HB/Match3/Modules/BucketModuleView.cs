using System;
using HB.Match3.Block;
using HB.Match3.Cell.Effect;
using HB.Match3.View;
using UnityEngine;

namespace HB.Match3.Modules
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
}