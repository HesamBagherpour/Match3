using UnityEngine;

namespace HB.Match3.Tutorial
{
    public class PowerUpTutorialStep : MonoBehaviour
    {
       
   
        private bool _isDone;
        public void Show()
        {
            if (_isDone) return;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _isDone = true;
            gameObject.SetActive(false);
        }
    }
}