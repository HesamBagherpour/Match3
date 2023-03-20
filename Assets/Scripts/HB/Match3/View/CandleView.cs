using UnityEngine;

namespace HB.Match3.View
{
    public class CandleView : MonoBehaviour
    {
        [SerializeField] GameObject candleOn;
        [SerializeField] Sprite candleOff;
        private SpriteRenderer spriteRenderer;
        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }
        public void SetState(bool state)
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (state)
            {
                spriteRenderer.enabled = false;
                candleOn.SetActive(true);
            }
            else
            {
                spriteRenderer.enabled = true;
                candleOn.SetActive(false);
            }
        }
    }
}