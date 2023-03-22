using HB.Match3.Block;
using HB.Match3.View;
using System;
using HB.Match3.Board;
using UnityEngine;

namespace HB.Match3.Modules
{
    public class LockModuleQuestView : ObjectModuleView
    {
        LockQuestView _lockQuestView;
        private BlockColor _color;

        public LockModuleQuestView(BaseModule module) : base(module)
        {
            Visible = true;
        }

        public override void Clear(Action onFinished)
        {
            Layer.Clear(this);
            var effect = Layer.PlayEffect(_lockQuestView.transform.position, "clear-lockQuest-" + _color.ToString().ToLower());
            BoardView.PlayAudio("chain-lock-break");
            effect.OnClear += () =>
            {
                onFinished?.Invoke();
            };
        }

        public override void SetGameObject(GameObject go)
        {
            _lockQuestView = go.GetComponent<LockQuestView>();
        }

        public void SetCount(int count)
        {
            _lockQuestView.SetCount(count);
        }

        internal void SetColor(BlockColor color)
        {
            _color = color;
            _lockQuestView.SetColor(color);
        }

        internal void PlayCollectEffect()
        {
            var effect = Layer.PlayEffect(_lockQuestView.transform.position, "impact-lockQuest-" + _color.ToString().ToLower());
            _lockQuestView.PlayTextShake();
        }
    }
}