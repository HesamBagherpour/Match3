using System;
using UnityEngine;

namespace HB.Core.Modules.GUI
{
    public class UiTransition : MonoBehaviour
    {
        #region Private Fields
        protected RectTransform rectTransform;

        private Action _onComplete;

        #endregion

        protected virtual void Awake()
        {
            rectTransform = transform as RectTransform;
        }

        #region Public Methods

        public void Apply(Action onComplete)
        {
            _onComplete = onComplete;
            OnApply();
        }

        #endregion

        #region Protected Methods

        protected virtual void OnApply()
        {
        }

        protected virtual void OnComplete()
        {
            _onComplete?.Invoke();
            _onComplete = null;
        }

        #endregion
    }
}