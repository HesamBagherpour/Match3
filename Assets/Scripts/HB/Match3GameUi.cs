using System;
using System.Collections.Generic;
using GameAnalyticsSDK;
using Garage.Analytic;
using Garage.Core.Modules.GUI;
using Garage.Decorator;
using Garage.Economy;
using Garage.HomeDesign.Utils;
using Garage.Match3;
using Garage.Match3.BoardStates;
using Garage.Match3.View.Quest;
using HB.Core.Modules.GUI;
using HB.Match3;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Garage.HomeDesign.Ui_Menus
{
    public class Match3GameUi : Window
    {
        public event Action PauseBtnClicked;
        public event Action<PowerUpData> PowerUpReleased;
        public event Action<PowerUpData> PowerUpSelected;

        public event Action<PowerUpData> PowerUpButtonClicked;
        public event Action<int> WinBtnClicked, LoseBtnClicked;

        [SerializeField] private GameObject _uiQuestItemPrefab;
        [SerializeField] private RectTransform _questParent;
        [SerializeField] private RectTransform _powerUpsParent;
        [SerializeField] private RectTransform _powerUpParentHeader;

        [SerializeField] private List<PowerUpButton> _powerUpButtons;
        [SerializeField] private RectTransform _coupons;
        [SerializeField] private Image _header;
        [SerializeField] private Sprite _normalLevelHeader;
        [SerializeField] private Sprite _hardLevelHeader;
        [SerializeField] private Sprite _luxuryLevelHeader;

        [SerializeField] private Image _Bg;
        [SerializeField] private Sprite _normalLevelBg;
        [SerializeField] private Sprite _hardLevelBg;
        [SerializeField] private Sprite _luxuryLevelBg;

        [SerializeField] private Button _pauseButton, _winGame, _loseGame;
        [SerializeField] private RTLTextMeshPro _remainingMoves, _totalCoupons;
        [SerializeField] private RTLTextMeshPro _levelNo;
        [SerializeField] private ParticleWrapper _addMovesEffect;
        [SerializeField] private List<PowerUpTutorialGroup> _powerUpTutorialGroups;
        private Wallet _wallet;

        private PuzzleQuestData _data;
        private QuestViewGenerator _viewGenerator;
        private static Dictionary<string, GameObject> _questItemsToShow;
        private List<QuestViewData> _quaViewDatas;
        public static Camera match3Camera;

        public void Init(BoardData boardData, PlayerProfileV2 profile, bool randomLevel, bool isLuxuryLevel, int extraMoves) //
        {
            match3Camera = GameObject.FindWithTag("match3_camera").GetComponent<Camera>();
            base.Init();
            CleanMenu();
            _data = boardData.questData;
            _wallet = profile?.Wallet;
            _levelNo.text = (boardData.index).ToString();
         
            if (boardData.isHardLevel)
            {
                _header.sprite = _hardLevelHeader;
                _Bg.sprite = _hardLevelBg;
            }
            else if(isLuxuryLevel)
            {
                _header.sprite = _luxuryLevelHeader;
                _Bg.sprite = _luxuryLevelBg;
            }
            else
            {
                _header.sprite = _normalLevelHeader;
                _Bg.sprite = _normalLevelBg;
            }


            _powerUpsParent.gameObject.SetActive(boardData.hasBooster);

            if (_questItemsToShow == null)
            {
                _questItemsToShow = new Dictionary<string, GameObject>();
            }

            _viewGenerator = new QuestViewGenerator();
            _questItemsToShow.Clear();
            _questItemsToShow = _viewGenerator.InitQuestView(_uiQuestItemPrefab, _questParent, _data.questDatas);
            _questItemsToShow.Add("coupon", _coupons.gameObject);

            _remainingMoves.text = (_data.totalMoves+extraMoves).ToString();
            _totalCoupons.text = "0";


            foreach (var group in _powerUpTutorialGroups)
            {
                // group.Done += PowerUpTutorialStep_Done;
                if (profile != null) group.Show(boardData.index, profile);
            }

            _powerUpTutorialGroups[0].transform.parent.position = _powerUpsParent.position;

            InitPowerUpBtns(boardData);
            if (profile != null)
                _pauseButton.gameObject.SetActive(profile.TutorialStepFinished >= 14);
            SwapState.EnterState -= EnablePauseButton;
            SwapState.EnterState += EnablePauseButton;
            SwapState.ExitState -= DisablePauseButton;
            SwapState.ExitState += DisablePauseButton;
        }

        private void InitPowerUpBtns(BoardData boardData)
        {
            if (!boardData.hasBooster) return;
            foreach (var pBtn in _powerUpButtons)
            {
                if (_wallet != null)
                {
                    var a = (int)_wallet.GetAmount(pBtn._data.CurrencyName);

                    pBtn.SetState(a);
                }

                pBtn.PowerUpButtonSelected += OnPowerUpButtonSelected;
                pBtn.PowerUpButtonReleased += PowerUpButtonReleased;

                switch (pBtn._data.CurrencyName)
                {
                    case CurrencyName.HammerPowerUp:
                        pBtn.SetAvailability(boardData.Hammer);
                        break;
                    case CurrencyName.InGameRainbowPowerUp:
                        pBtn.SetAvailability(boardData.InGameRainbow);
                        break;
                    case CurrencyName.InGameBlasterPowerUp:
                        pBtn.SetAvailability(boardData.InGameBlaster);
                        break;

                }
            }
        }

        private void EnablePauseButton()
        {
            _pauseButton.interactable = true;
        }

        private void DisablePauseButton()
        {
            _pauseButton.interactable = false;
        }

        public void SetChallengeState(bool isChallengeAvailable)
        {

            _coupons.gameObject.SetActive(isChallengeAvailable);
            _powerUpParentHeader.gameObject.SetActive(isChallengeAvailable);

        }
        public static Vector2 QuestItemPosition(string questName)
        {
            if (_questItemsToShow?.ContainsKey(questName) == true)
            {
                Vector2 pos = _questItemsToShow[questName].transform.position;
                return pos;
                //return match3Camera.ScreenToWorldPoint(pos);
            }
            else
            {
                return new Vector2(-4, 0);
            }

        }

        public override void OnBackBtnPressed()
        {
            base.OnBackBtnPressed();
            OnPauseBtnClicked();
        }

        public void OnEnable()
        {

            _pauseButton.onClick.AddListener(OnPauseBtnClicked);
        }

        public void OnDisable()
        {
            SwapState.EnterState -= EnablePauseButton;
            SwapState.ExitState -= DisablePauseButton;

            _pauseButton.onClick.RemoveListener(OnPauseBtnClicked);
        }

        private void OnPauseBtnClicked()
        {
         
            PauseBtnClicked?.Invoke();
        }

        private void PowerUpButtonReleased(PowerUpData data)
        {
            PowerUpReleased?.Invoke(data);

        }
        private void OnPowerUpButtonSelected(PowerUpData data)
        {
            PowerUpSelected?.Invoke(data);
        }
        public void SelectPowerUp(PowerUpData data)
        {
            var pu = _powerUpButtons.Find(p => p._data == data);
            pu.SetState(-1);
            foreach (var group in _powerUpTutorialGroups)
            {
                group.PowerUpBtnSelected(data);
            }
        }
        public void UnSelectPowerUp(PowerUpData data)
        {
            var pu = _powerUpButtons.Find(p => p._data == data);
            var amount = (int)_wallet.GetAmount(data.CurrencyName);
            pu.SetState(amount);
        }
        public void UsePowerUp(PowerUpData data)
        {
            //AdjustWrapper.SendAdjustEvent(boardData.PowerUpUsedAdjustEvent);
            GameAnalytics.NewDesignEvent("Design_PowerUps_" + data.BoosterType + "_Used");
            var a = _wallet.GetAmount(data.CurrencyName);
            UnSelectPowerUp(data);
            for (var index = 0; index < _powerUpTutorialGroups.Count; index++)
            {
                _powerUpTutorialGroups[index].Finish();
            }
        }
        public override void Close()
        {
            CleanMenu();

            foreach (var pBtn in _powerUpButtons)
            {
                pBtn.PowerUpButtonSelected -= OnPowerUpButtonSelected;
                pBtn.PowerUpButtonReleased -= PowerUpButtonReleased;
            }

            base.Close();
        }
        private void CleanMenu()
        {
            if (_questItemsToShow != null)
                foreach (var qItem in _questItemsToShow)
                {
                    if (qItem.Key != "coupon")
                        Destroy(qItem.Value);
                }

            _questItemsToShow?.Clear();

        }
        private void LoseGame()
        {
            LoseBtnClicked?.Invoke(1);
        }
        private void WinGame()
        {
            WinBtnClicked?.Invoke(700);
        }

        List<float> remainingTimes = new List<float>();
        List<MoveData> remainingMoveDatas = new List<MoveData>();
        public void UpdateOnMatches(MoveData data)
        {
            _remainingMoves.text = data.RemainingMoves.ToString();
            remainingTimes.Add(1f);
            remainingMoveDatas.Add(data);
            _totalCoupons.text = data.TotalCoupons.ToString();
        }

        private void Update()
        {
            if (remainingTimes.Count == 0) return;
            for (int i = 0; i < remainingTimes.Count; i++)
            {
                remainingTimes[i] -= Time.deltaTime;
            }
            if (remainingTimes[0] <= 0f)
            {
                foreach (var quest in _questItemsToShow)
                {
                    foreach (var remaining in remainingMoveDatas[0].RemainingQuests)
                    {
                        if (quest.Key == remaining.QuestName)
                        {
                            quest.Value.GetComponent<UiQuestElement>().SetCount(remaining.count);
                        }
                    }
                }
                remainingMoveDatas.RemoveAt(0);
                remainingTimes.RemoveAt(0);
            }
        }

        public void AddMoves(int extraMoves)
        {
            _remainingMoves.text = extraMoves.ToString();
            _addMovesEffect.Play(null);
        }
    }
}