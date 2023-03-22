using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HB.Packages.GUI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class Window : UiWidget
    {
        #region Private Fields

        [SerializeField] private List<Window> modalWindows = new List<Window>();
        [SerializeField] private List<UiWidget> widgets = new List<UiWidget>();
        private WindowCollection _windows;

        private Canvas _canvas;
        private int _closedWidgetCount;

        #endregion

        #region Public Methods

        public override void Init()
        {
            base.Init();
            if (rectTransform != null) _canvas = rectTransform.GetComponent<Canvas>();
            if (_windows == null)
            {
                Debug.Assert(modalWindows != null, $"{this}: modals are null!");

                _windows = new WindowCollection(_canvas, modalWindows, false);
            }
        }

        public TWindow OpenModal<TWindow>() where TWindow : Window
        {
            TWindow wnd = _windows.OpenWindow<TWindow>();
            wnd.Open();
            return wnd;
        }

        public TWindow GetModal<TWindow>() where TWindow : Window
        {
            TWindow wnd = _windows.OpenWindow<TWindow>();
            wnd.Init();
            return wnd;
        }

        public void Close<TWindow>() where TWindow : Window
        {
            _windows.Close<TWindow>();
        }

        #endregion

        #region Protected Methods

        protected override void OnOpened()
        {
            base.OnOpened();
            _closedWidgetCount = widgets.Count;

            for (int i = 0; i < widgets.Count; i++)
            {
                widgets[i].Opened += OnWidgetOpened;
                widgets[i].Open();
            }
        }

        protected override void OnClosed()
        {
            _closedWidgetCount = widgets.Count;
            if (_closedWidgetCount == 0) base.OnClosed();

            for (int i = 0; i < widgets.Count; i++)
            {
                widgets[i].Close();
                widgets[i].Closed += OnWidgetClosed;
            }

            _windows?.CloseAll();
        }

        private void OnWidgetClosed(UiWidget widget)
        {
            widget.Opened -= OnWidgetOpened;
            _closedWidgetCount--;

            if (_closedWidgetCount == 0) base.OnClosed();
        }

        #endregion

        #region Private Methods

        private void OnWidgetOpened(UiWidget widget)
        {
            widget.Opened -= OnWidgetOpened;
            _closedWidgetCount--;

            if (_closedWidgetCount == 0) base.OnOpened();
        }

        #endregion

        public virtual void OnBackBtnPressed()
        {

        }
    }
}