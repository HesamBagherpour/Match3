using System;
using Garage.Core.Modules.GUI;
using UnityEngine;

namespace HB.Core.Modules.GUI
{
    public class UiWidget : MonoBehaviour, IWidget
    {
        #region Events

        public event Action<UiWidget> Opened;
        public event Action<UiWidget> Closed;

        #endregion

        #region Protected Fields

        protected RectTransform rectTransform;

        [SerializeField] private UiTransition openUiTransition;
        [SerializeField] private UiTransition closeUiTransition;

        #endregion

        #region IWidget Interface

        public virtual void Init()
        {
            rectTransform = transform as RectTransform;
        }

        public virtual void Open()
        {

            gameObject.SetActive(true);
            if (openUiTransition != null)
                openUiTransition.Apply(OnOpened);
            else
                OnOpened();
        }

        public virtual void Close()
        {

            if (gameObject.activeInHierarchy == false) return;
            if (closeUiTransition != null)
                closeUiTransition.Apply(OnClosed);
            else
                OnClosed();
        }

        #endregion

        #region Protected Methods

        protected virtual void OnOpened()
        {
            Opened?.Invoke(this);
        }

        protected virtual void OnClosed()
        {

            gameObject.SetActive(false);
            Closed?.Invoke(this);
        }

        #endregion
    }
}