using System;
using UnityEngine;

namespace HB.Packages.Utilities
{
    public class ClickableObject : MonoBehaviour
    {
        public bool is3D = false;
        public string[] maskLayers = null;
        private bool _isDown = false;
        public event Action Pressed = null;
        public event Action Released = null;
        public event Action Clicked = null;

        private void Awake()
        {
            if (ClickableObjectSystem.Instance != null) 
                ClickableObjectSystem.Instance.AddClickableObject(this);
        }

        private void OnDestroy()
        {
            ClickableObjectSystem.Instance?.RemoveClickableObject(this);
        }
        internal void CallMousePress()
        {
            _isDown = true;
            Pressed?.Invoke();
        }
        internal void CallMouseRelease()
        {
            bool isClick = _isDown;
            if (_isDown)
            {
                _isDown = false;
                Released?.Invoke();
            }

            if (isClick)
            {
                Clicked?.Invoke();
            }
        }
        internal void HandleMouseRelease()  //Called When mouse release in happened but mouse is not over this object
        {
            if (_isDown)
            {
                _isDown = false;
                Released?.Invoke();
            }
        }
    }
}
