using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Garage.Match3.Cells;
using HB.Match3.View;
using HB.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;


namespace Garage.Match3.View
{
    public static class ListShuffleExtiontion
    {
        private static System.Random rng = new System.Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
    public class BoosterObjectLayer : ObjectLayer
    {
        public BoosterObjectLayer(int order, string name, TilemapSettings tilemapSettings, BoardViewData viewData,
            LayerStack layerStack) : base(order, name, tilemapSettings, viewData, layerStack)
        {
        }
    }

    public class BlockObjectLayer : ObjectLayer
    {
        private GameObject _sweepEffect;
        private const string SweepPrefabName = "VFXSweep";

        public BlockObjectLayer(int order, string name, TilemapSettings tilemapSettings, BoardViewData viewData,
            LayerStack layerStack) :
            base(
                order,
                name,
                tilemapSettings,
                viewData,
                layerStack)
        {
            GameObject sweepEffectPrefab = viewData.layerViewData.allPrefabs.Find(go => go.name == SweepPrefabName);
            _sweepEffect = Object.Instantiate(sweepEffectPrefab);
            _sweepEffect.gameObject.SetActive(false);
        }

        public void Sweep(Vector3 pos, Vector3 targetPosition)
        {
            if (_sweepEffect != null)
            {
                _sweepEffect.SetActive(true);
                _sweepEffect.transform.position = pos;
                _sweepEffect.transform.LookAt(targetPosition);
                ParticleSystem ps = _sweepEffect.GetComponent<ParticleSystem>();
                ps.Play();
            }
        }
    }

    public class ObjectLayer : IBoardLayer
    {
        private readonly ObjectPool<GameObject> _objectPool;
        private readonly LayerStack _layerStack;
        private readonly GameObject _prefab;
        private static readonly Dictionary<string, GameObject> IdToPrefabs = new Dictionary<string, GameObject>();
        private readonly Grid _grid;
        private Vector2Int _boardOffset;
        private Action onLayerShowComplete;
        private int scaleUpCounter;
        private readonly Transform _parent;

        private readonly Dictionary<IModuleView, GameObject> _activeObjects = new Dictionary<IModuleView, GameObject>();
        public Transform Grid => _grid.transform;
        public int ActiveCells { get; private set; }

        public ObjectLayer(int order, string name, TilemapSettings tilemapSettings, BoardViewData viewData,
            LayerStack layerStack)
        {
            _layerStack = layerStack;
            _prefab = viewData.layerViewData.allPrefabs.Find(go => go.name == name);
            if (_prefab == null)
            {
                Debug.LogError("Could not find game object with name " + name);
            }
            _grid = tilemapSettings.grid;
            _boardOffset = tilemapSettings.boardOffset;
            _parent = new GameObject(name + "-layer").transform;
            _parent.SetParent(_grid.transform, false);

            AddToIdTable(viewData);

            _objectPool = new ObjectPool<GameObject>(50,
                InstantiateAndAttach,
                GetPooledView,
                ReleaseView,
                $"{name} Pool");

            _objectPool.WarmUp(1);
        }

        private static void AddToIdTable(BoardViewData viewData)
        {
            for (int i = 0; i < viewData.layerViewData.allPrefabs.Count; i++)
            {
                GameObject go = viewData.layerViewData.allPrefabs[i];
                if (IdToPrefabs.ContainsKey(go.name) == false)
                {
                    IdToPrefabs.Add(go.name, go);
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void SetTile(int x, int y, IModuleView moduleView)
        {
            ActiveCells++;
            if (!moduleView.Visible) return;
            GameObject go = Spawn(x, y);
            go.name = $"{moduleView}({x},{y})";
            ObjectModuleView view = (ObjectModuleView)moduleView;
            _activeObjects.Add(view, go);
            view.SetGameObject(go);
        }

        public void Setup(TilemapSettings tilemapSettings)
        {
            _boardOffset = tilemapSettings.boardOffset;
        }


        #region Private Methods

        private static void ReleaseView(GameObject go)
        {

            if (go.activeInHierarchy)
                go.SetActive(false);

        }

        private static void GetPooledView(GameObject go)
        {
            go.gameObject.SetActive(true);
        }

        public TileBase GetTile(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void Clear(IModuleView module)
        {
            Recycle(_activeObjects[module]);
            _activeObjects.Remove(module);
            ActiveCells--;
        }

        public virtual Effect PlayEffect(Vector3 pos, string id)
        {
            return _layerStack.PlayEffect(pos, id);
        }
        public Effect PlayEffect(Vector3 pos, Vector3 targetPos, string id)
        {
            return _layerStack.PlayEffect(pos, targetPos, id);
        }

        public virtual bool ReleaseEffect(Effect effect)
        {
            return _layerStack.ReleaseEffect(effect);
        }

        private GameObject InstantiateAndAttach()
        {
            GameObject blockInstance = Object.Instantiate(
                _prefab,
                _parent,
                false);

            blockInstance.layer = BoardView.Match3BoardLayerIndex;
            blockInstance.gameObject.SetActive(false);
            return blockInstance;
        }

        internal GameObject Spawn(int x, int y)
        {
            GameObject go = _objectPool.Get();
            go.transform.position = CellToWorld(new Point(x, y));
            return go;
        }

        public void Recycle(GameObject go)
        {
            _objectPool.Release(go);
        }

        public Vector3 CellToWorld(Point pos)
        {
            return _layerStack.CellToWorld(pos);
        }

        public void Hide()
        {
            foreach (var _object in _activeObjects)
            {
                _object.Value.SetActive(false);
            }
        }

        public async void Show(Action onComplete)
        {
            if (ActiveCells == 0)
            {
                onComplete?.Invoke();
                return;
            }
            onLayerShowComplete = onComplete;
            scaleUpCounter = 0;
            var tmpObjects = new List<GameObject>();
            foreach (var _object in _activeObjects)
            {
                tmpObjects.Add(_object.Value);
            }
            tmpObjects.Shuffle();

            foreach (var go in tmpObjects)
            {
                go.SetActive(true);
                go.transform.localScale = Vector3.zero;
                scaleUpCounter++;
                var scaleUpTween = go.transform.DOScale(Vector3.one, 0.7f);
                scaleUpTween.SetEase(Ease.OutElastic, 0.3f);
                scaleUpTween.onComplete += ScaleUpComplete;
                await UniTask.Delay(5);
            }
        }

        private void ScaleUpComplete()
        {
            scaleUpCounter--;
            if (scaleUpCounter == 0)
            {
                onLayerShowComplete?.Invoke();
            }
        }
        #endregion
    }
}