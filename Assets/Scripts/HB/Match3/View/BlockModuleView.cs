using System;
using System.Collections.Generic;
using HB.Match3.Block;
using HB.Match3.Cell.Effect;
using HB.Match3.Modules;
using UnityEngine;

namespace HB.Match3.View
{
    public class BlockModuleView : ObjectModuleView
    {
        private BlockView _blockView;
        private BlockType _blockType;
        private SpriteRenderer _spriteRenderer;
        private static Dictionary<BlockType, Sprite> _blockTypeToSprite;
        private Effect jumboEffect;
        private bool hasCoupon;
        private static Sprite _couponSprite;
        private Effect plantAnimation;
        private bool _mergeIntoOther;
        
        public BlockModuleView(BaseModule module) : base(module)
        {
        }

        public override void SetGameObject(GameObject go)
        {
            throw new NotImplementedException();
        }

        public override void Clear(Action onFinished)
        {
            throw new NotImplementedException();
        }
    }
}