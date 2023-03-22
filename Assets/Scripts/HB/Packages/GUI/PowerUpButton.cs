using HB.Match3.DataManagement;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

namespace HB.Packages.GUI
{
    public class PowerUpButton : MonoBehaviour
    {
        public event Action<PowerUpData> PowerUpButtonSelected;
        public event Action<PowerUpData> PowerUpButtonReleased;
        public PowerUpData _data;
        [SerializeField] private Button _btn;
        private bool _isSelected;
        [SerializeField] private Image _backGround, _powerUpImage, _lock;
        [SerializeField] private RectTransform _notAvailable, _available, _activated;
        [SerializeField] private TextMeshPro _amountText;
        [SerializeField] private GameObject _selectionEffect;
        private int _amount;
        
        private void Awake()
        {
            _powerUpImage.sprite = _data.PowerUpImage;
        }

        private void OnEnable()
        {
            _btn.onClick.AddListener(OnPowerUpButtonClicked);
        }

        private void OnDisable()
        {
            _btn.onClick.RemoveListener(OnPowerUpButtonClicked);
        }

        public void SetAvailability(bool isActive)
        {
            _btn.interactable = isActive;
            _lock.gameObject.SetActive(!isActive);
            if (!isActive)
            {

                _available.gameObject.SetActive(false);
                _notAvailable.gameObject.SetActive(false);
                _activated.gameObject.SetActive(false);
            }
        }

        public void SetState(int amount)
        {
            _amountText.text = (amount > 0 ? amount : 0).ToString();
            _available.gameObject.SetActive(amount > 0);
            _notAvailable.gameObject.SetActive(amount == 0);
            _activated.gameObject.SetActive(amount < 0);
            _selectionEffect.SetActive(amount < 0);
            if (amount >= 0)
            {
                _isSelected = false;
            }
            else
            {
                _isSelected = true;
            }
        }

        protected virtual void OnPowerUpButtonClicked()
        {

            if (_isSelected)
            {

                PowerUpButtonReleased?.Invoke(_data);
            }
            else
            {

                PowerUpButtonSelected?.Invoke(_data);
            }

        }

    }
}