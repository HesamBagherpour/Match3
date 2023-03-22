using System;
using System.Collections.Generic;
using HB.Match3.DataManagement;
using UnityEngine;
using UnityEngine.UI;

namespace HB.Match3.Tutorial
{
    public class PowerUpTutorialGroup : MonoBehaviour
    {
        public event Action Opened;
        public event Action<int> Done;


        [SerializeField] private List<PowerUpTutorialStep> _steps;
        [SerializeField] private int _match3LevelToShow;
        [SerializeField] private PowerUpData _powerUpType;
        [SerializeField] private Button _okBtn;
        private PlayerProfileV2 _profile;
        private bool _isActive = false;

        private void OnEnable()
        {
            _okBtn.onClick.AddListener(OkBtnClicked);
        }

        private void OnDisable()
        {
            _okBtn.onClick.RemoveListener(OkBtnClicked);
        }

        public PowerUpData Show(int match3Level, PlayerProfileV2 profile)
        {
            if (match3Level != _match3LevelToShow)
            {
                foreach (var step in _steps)
                {
                    step.gameObject.SetActive(false);
                }
                return null;
            }
            if (profile.IntVariables.ContainsKey(_powerUpType.CurrencyName + "_tut")) return null;
            _profile = profile;
            _isActive = true;
            _steps[0].Show();
            return _powerUpType;

        }

        private void OkBtnClicked()
        {
            if (!_profile.StringVariables.ContainsKey("ActiveTutorialPowerup"))
                _profile.StringVariables.Add("ActiveTutorialPowerup", _powerUpType.CurrencyName.ToString());
            else
                _profile.StringVariables["ActiveTutorialPowerup"] = _powerUpType.CurrencyName.ToString();

            _profile.IntVariables.Add(_powerUpType.CurrencyName + "_tut", 1);
            if (_profile.Wallet.GetAmount(_powerUpType.CurrencyName) <= 0)
                _profile.Wallet.Earn(CurrencyName.Gem, _powerUpType.PriceByGem, "Tutorial");
            _steps[0]?.Hide();
            _steps[1]?.Show();
        }

        public void PowerUpBtnSelected(PowerUpData data)
        {
            if (data.CurrencyName != _powerUpType.CurrencyName) return;
            if (!_isActive) return;
            _steps[1]?.Hide();
            _steps[2]?.Show();

        }


        public void Finish()
        {
            if (!_isActive) return;
            _isActive = false;
            _steps[2]?.Hide();

        }
    }
}