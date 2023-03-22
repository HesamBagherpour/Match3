using System.Collections.Generic;
using UnityEngine;
using System;

namespace HB.Packages.GUI
{
    public class WindowCollection
    {
        private readonly Dictionary<Type, Window> _windows;
        private readonly Dictionary<Type, Window> _openedWindows;
        private readonly Canvas _canvasRoot;

        public WindowCollection(Canvas canvasRoot, List<Window> windows, bool useWindowsAsModel)
        {
            _canvasRoot = canvasRoot;
            _windows = new Dictionary<Type, Window>();

            for (int i = 0; i < windows.Count; i++)
            {
                _windows.Add(windows[i].GetType(), windows[i]);
            }

            _openedWindows = useWindowsAsModel ? new Dictionary<Type, Window>(_windows.Count) : _windows;
        }

        public TWindow OpenWindow<TWindow>() where TWindow : Window
        {
            Type type = typeof(TWindow);
            _openedWindows.TryGetValue(type, out Window wnd);
            if (wnd != null) return wnd as TWindow;
            _windows.TryGetValue(type, out wnd);
            Window instance = UnityEngine.Object.Instantiate(wnd, Vector3.zero, Quaternion.identity);
            UnityEngine.Object.DontDestroyOnLoad(instance);
            instance.transform.SetParent(_canvasRoot.transform, false);
            //RectTransform rect = instance.GetComponent<RectTransform>();
            //rect.sizeDelta= new Vector2(Screen.width, Screen.height);
            instance.Init();
            if (instance != null) _openedWindows.Add(type, instance);
            return instance as TWindow;
        }

        public void Close<TWindow>() where TWindow : Window
        {
            Type type = typeof(TWindow);
            _openedWindows[type].Close();
        }

        public void CloseAll()
        {
            foreach (KeyValuePair<Type, Window> openedWindow in _openedWindows)
            {
                for (int i = 0; i < _openedWindows.Count; i++)
                {
                    openedWindow.Value.Close();
                }
            }
        }
    }
}