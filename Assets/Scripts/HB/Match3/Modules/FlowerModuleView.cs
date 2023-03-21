using HB.Match3.View;
using System;
using HB.Match3.Block;
using HB.Match3.Cell.Effect;
using UnityEngine;

namespace HB.Match3.Modules
{
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