using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HB.Utilities.ClickableObject
{
    public class ClickableObjectSystem : MonoBehaviour
    {
        private static ClickableObjectSystem _instance = null;
        public static ClickableObjectSystem Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("ClickableSystem").AddComponent<ClickableObjectSystem>();
                }
                return _instance;
            }
        }

        public class ClickableHandleSystem
        {
            protected readonly List<ClickableObject> clickableObjects = new List<ClickableObject>();
            protected readonly List<int> masks = new List<int>();
            protected readonly Dictionary<int, int> masksObjectCount = new Dictionary<int, int>();
            protected Camera camera = null;

            public void AddClickableObject(ClickableObject obj)
            {
                clickableObjects.Add(obj);
                int mask = GetMask(obj);
                AddObjectToMask(mask);
            }
            public void RemoveClickableObject(ClickableObject obj)
            {
                clickableObjects.Remove(obj);
                int mask = GetMask(obj);
                RemoveObjectFromsMask(mask);
            }

            private int GetMask(ClickableObject obj)
            {
                int mask;
                if (obj.maskLayers == null || obj.maskLayers.Length <= 0)
                    mask = -1;
                else mask = LayerMask.GetMask(obj.maskLayers);
                return mask;
            }

            public void AddObjectToMask(int mask)
            {
                if (!masksObjectCount.ContainsKey(mask))
                {
                    masksObjectCount.Add(mask, 1);
                    masks.Add(mask);
                }
                else
                {

                    masksObjectCount[mask]++;
                }
            }

            public void RemoveObjectFromsMask(int mask)
            {
                if (masksObjectCount.ContainsKey(mask))
                {
                    masksObjectCount[mask]--;
                    if (masksObjectCount[mask] <= 0)
                    {
                        masksObjectCount.Remove(mask);
                        masks.Remove(mask);
                    }
                }
            }

            public void Update()
            {
                if (Input.GetMouseButtonDown(0))
                {
                    List<ClickableObject> objList = GetHitObjects();
                    for (int i = 0; i < objList.Count; i++)
                        objList[i].CallMousePress();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    List<ClickableObject> objList = GetHitObjects();
                    for (int i = 0; i < objList.Count; i++)
                        objList[i].CallMouseRelease();

                    for (int i = 0; i < clickableObjects.Count; i++)
                        clickableObjects[i].HandleMouseRelease();
                }
            }

            public virtual List<ClickableObject> GetHitObjects()
            {
                return null;
            }
            public void SetCamera(Camera camera, bool handle3D = true, bool handle2D = true)
            {
                this.camera = camera;
            }

            public bool IsMouseOverUi()
            {
                var mPointerEventData = new PointerEventData(EventSystem.current);
                mPointerEventData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(mPointerEventData, results);

                return (results.Count > 0);
            }

        }

        public class ClickableHandleSystem3D : ClickableHandleSystem
        {
            public override List<ClickableObject> GetHitObjects()
            {
                List<ClickableObject> objList = new List<ClickableObject>();
                //if (!EventSystem.current.IsPointerOverGameObject() || EventSystem.current.currentSelectedGameObject == null)
                if (IsMouseOverUi()) return objList;
                for (int i = 0; i < masks.Count; i++)
                {
                    int mask = masks[i];
                    Ray ray = (camera != null ? camera : Camera.main).ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if ((mask == -1 || !Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) &&
                        !Physics.Raycast(ray, out hit, Mathf.Infinity)) continue;
                    if (hit.collider == null || hit.collider.gameObject == null) continue;
                    ClickableObject obj = hit.collider.gameObject.GetComponent<ClickableObject>();
                    if (obj != null)
                        objList.Add(obj);
                }

                return objList;
            }

        }
        public class ClickableHandleSystem2D : ClickableHandleSystem
        {
            public override List<ClickableObject> GetHitObjects()
            {
                List<ClickableObject> objList = new List<ClickableObject>();
                if (IsMouseOverUi()) return objList;
                for (int i = 0; i < masks.Count; i++)
                {
                    int mask = masks[i];
                    Ray ray = (camera != null ? camera : Camera.main).ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D hit;
                    if (mask == -1)
                        hit = Physics2D.Raycast(ray.origin, ray.direction);
                    else
                        hit = Physics2D.Raycast(ray.origin, ray.direction, mask);

                    if (hit.collider == null) continue;

                    if (hit.collider == null || hit.collider.gameObject == null) continue;
                    ClickableObject obj = hit.collider.gameObject.GetComponent<ClickableObject>();
                    if (obj != null)
                        objList.Add(obj);

                }

                return objList;
            }

        }

        private ClickableHandleSystem2D _handleSystem2D = null;
        private ClickableHandleSystem3D _handleSystem3D = null;
        private Camera _camera = null;
        [SerializeField] private bool _handle3D = true;
        [SerializeField] private bool _handle2D = true;

        void Awake()
        {
            if (_instance == null)
                _instance = this;
            DontDestroyOnLoad(gameObject);
            Input.simulateMouseWithTouches = true;

            _handleSystem2D = new ClickableHandleSystem2D();
            _handleSystem3D = new ClickableHandleSystem3D();
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        void Update()
        {
            if (_handle2D)
                _handleSystem2D.Update();
            if (_handle3D)
                _handleSystem3D.Update();
        }

        public void AddClickableObject(ClickableObject obj)
        {
            if (obj.is3D)
                _handleSystem3D.AddClickableObject(obj);
            else
                _handleSystem2D.AddClickableObject(obj);
        }
        public void RemoveClickableObject(ClickableObject obj)
        {
            if (obj.is3D)
                _handleSystem3D?.RemoveClickableObject(obj);
            else
                _handleSystem2D?.RemoveClickableObject(obj);
        }

        public void SetCamera(Camera camera, bool handle3D = true, bool handle2D = true)
        {
            this._camera = camera;
            _handleSystem2D.SetCamera(camera);
            _handleSystem3D.SetCamera(camera);
            this._handle3D = handle3D;
            this._handle2D = handle2D;
        }
    }
}
