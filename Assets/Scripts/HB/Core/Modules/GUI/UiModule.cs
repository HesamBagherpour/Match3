using System;
using System.Collections.Generic;
//using Garage.Core.AssetStreaming;
using Garage.Core.DI;
using Garage.Core.Modules.GUI;
using HB.Core.DI;
using UnityEngine;
using IModule = HB.Match3.Cells.IModule;

namespace HB.Core.Modules.GUI
{
    public class UiModule : MonoModule, ILoadable
    {
        [SerializeField] private RectTransform header;
        [SerializeField] private RectTransform footer;
        private List<Window> _windowModels;
        private WindowCollection _windows;
        private IContext _context;

        //private AssetLoader _assetLoader;
        private bool _isLoaded = false;
        private bool _initialized = false;
        public Canvas CanvasRoot { get; private set; }

        public override void OnRegister(IContext context)
        {
            if (_initialized) return;
            base.OnRegister(context);
            _context = context;
           // _assetLoader = _context.Get<AssetLoader>();
            _initialized = true;
        }

        public override void Init()
        {
            base.Init();
            _windows = new WindowCollection(CanvasRoot, _windowModels, true);
        }

        public T OpenWindow<T>() where T : Window
        {
            T wnd = _windows.OpenWindow<T>();
            return wnd;
        }

        public T OpenInHeader<T>() where T : Window
        {
            Window wnd = OpenWindow<T>();
            wnd.transform.SetParent(header);
            return (T) wnd;
        }

        public T OpenInFooter<T>() where T : Window
        {
            Window wnd = OpenWindow<T>();
            wnd.transform.SetParent(footer);
            return (T) wnd;
        }

        public void CloseWindow<T>() where T : Window
        {
            _windows.Close<T>();
        }

        public async void Load(Action<IModule> onLoaded)
        {
            if (_isLoaded)
            {
                onLoaded?.Invoke(this);
                return;
            }

            // IList<GameObject> wins = await _assetLoader.LoadAssets<GameObject>("Window");
            IList<GameObject> wins = Resources.LoadAll<GameObject>("Ui/Windows");
            
            GameObject canvasPrefab = Resources.Load<GameObject>("CanvasRoot");
            int count = 0;
            while (canvasPrefab == null)
            {
                Debug.LogError("loaded canvas root returned null try to failsafe: " + ++count);
                canvasPrefab = Resources.Load<GameObject>("CanvasRoot");
                if(count > 10)
                    Debug.LogError("Fatal Problem not solved by safe check");
            }
            if(count > 0 && canvasPrefab != null)
                Debug.LogError("successful fail safe maneuvre =D");
            GameObject canvasGo = Instantiate(canvasPrefab, Vector3.zero, Quaternion.identity);
            CanvasRoot = canvasGo.GetComponent<Canvas>();
            DontDestroyOnLoad(CanvasRoot);
            _windowModels = new List<Window>(wins.Count);
            for (int i = 0; i < wins.Count; i++)
            {
                Window win = wins[i].GetComponent<Window>();
                if (win == null)
                {
                    Debug.LogError($"Wrong asset marked as Window: {wins[i].name}");
                }

                _windowModels.Add(win);
            }

            _isLoaded = true;
            onLoaded?.Invoke(this);
        }
    }
}