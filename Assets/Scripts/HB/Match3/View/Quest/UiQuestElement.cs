using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HB.Match3.View.Quest
{
    public class UiQuestElement : MonoBehaviour
    {
        [SerializeField] private Image _doneCheck;
        [SerializeField] private TextMeshPro _questCount;
        [SerializeField] private ParticleSystem _hitParticle;
        private int _count;
        public void Init(int count)
        {
            _doneCheck.gameObject.SetActive(false);
            SetCount(count);

        }

        private void QuestDone()
        {

            _doneCheck.gameObject.SetActive(true);
            _questCount.gameObject.SetActive(false);
        }

        public void SetCount(int count)
        {
            if (_count == count) return;
            _count = count;
            _questCount.text = count.ToString();
            _hitParticle.Play();
            if (_count <= 0)
            {
                QuestDone();
            }

        }


    }
}
