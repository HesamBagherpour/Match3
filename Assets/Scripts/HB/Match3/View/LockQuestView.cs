using DG.Tweening;
using HB.Match3.Block;
using TMPro;
using UnityEngine;

namespace HB.Match3.View
{
    public class LockQuestView : MonoBehaviour
    {
        [SerializeField] TextMeshPro textMeshComponent;
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Sprite redLock;
        [SerializeField] Sprite greenLock;
        [SerializeField] Sprite cyanLock;
        [SerializeField] Sprite purpleLock;
        [SerializeField] Sprite yellowLock;
        [SerializeField] Sprite orangeLock;
        private Vector3 startScale;
        
        
        private void Awake()
        {
            startScale = textMeshComponent.transform.localScale;
        }
        public void SetColor(BlockColor color)
        {
            switch (color)
            {
                case BlockColor.Red:
                    spriteRenderer.sprite = redLock;
                    break;
                case BlockColor.Green:
                    spriteRenderer.sprite = greenLock;
                    break;
                case BlockColor.Cyan:
                    spriteRenderer.sprite = cyanLock;
                    break;
                case BlockColor.Purple:
                    spriteRenderer.sprite = purpleLock;
                    break;
                case BlockColor.Yellow:
                    spriteRenderer.sprite = yellowLock;
                    break;
                case BlockColor.Orange:
                    spriteRenderer.sprite = orangeLock;
                    break;
                case BlockColor.None:
                default:
                    break;
            }
        }

        internal void PlayTextShake()
        {
            textMeshComponent.transform.DOShakeScale(0.3f, 1, 10, 90, true).onComplete += () =>
            {
                textMeshComponent.transform.DOScale(startScale, 0.1f);
            };
        }

        public void SetCount(int count)
        {
            textMeshComponent.text = count.ToString();
        }
    }
}