using System;
using System.Collections.Generic;
using Garage.Match3;
using Garage.Match3.Cells.Modules;
using Garage.Match3.View;
using HB.Match3.View;
using HB.StateMachine;
using HB.Timers;
using HB.Utilities;
using UnityEngine;

namespace HB.Match3
{
    public class PuzzleController : Controller
    {
        public abstract class PuzzleState : State<PuzzleController>
        {
            protected static PuzzleStateContext stateContext;
        }

        public abstract class PuzzleStateContext
        {
        }

        public class ResultStateContext : PuzzleStateContext
        {
            public Match3Result PlayResult;
        }


        public class InitState : PuzzleState
        {
            //private AudioPlayer audioPlayer;

            protected override void OnEnter()
            {
                //Agent.gameObject.SetActive(true);

                base.OnEnter();
                Agent.Restart = false;
                Agent.ExitWithoutPlay = false;
                stateContext = null;
                Init();
            }

            public void Init()
            {
                // Debug.Log($"InitState: {Agent._boardData.width},{Agent._boardData.height}");
                AssetLoader loader = null;

                MonoBehaviour.print("initilization puzzle controller 222222222222222222");
                AudioModule audioModule = Context?.Get<AudioModule>();
                if (audioModule != null) audioPlayer = audioModule.GetAudioPlayer();

#if UNITY_EDITOR
                if (audioPlayer == null) audioPlayer = GameObject.FindObjectOfType<AudioPlayer>();
#endif

#if UNITY_EDITOR
                loader = Context == null ? new AssetLoader() : Context.Get<AssetLoader>();
                loader.Load(_ => InitBoardView(loader));
#else
                loader = Context.Get<AssetLoader>();
                if (loader != null)
                {
                    InitBoardView(loader);
                }
#endif
            }

            private async void InitBoardView(AssetLoader loader)
            {
                MonoBehaviour.print("initilization board view 11111111111111111111");
                BoardViewData data = Resources.Load<BoardViewData>("Match3/StaticAssets/board-view-data");
                CreateBoardViewIfNotExist();
                Agent._boardData.Deserialize();
                Agent.boardView.SetViewData(data, audioPlayer);
                Agent._board = new Board(Agent._boardData, Agent.boardView);
                
                


                QuestGiver.SetData(Agent._board, Agent.boardView, Agent._boardData.questData);
                //Debug.Log("Init QuestGiver");
                Finished();
            }

            private void CreateBoardViewIfNotExist()
            {
                if (Agent.boardView == null)
                {
                    GameObject go = new GameObject("BoardView", typeof(BoardView));
                    go.transform.SetParent(Agent.transform, false);
                    Agent.boardView = go.GetComponent<BoardView>();
                }
            }
        }

        public class EnterState : PuzzleState
        {
            private GameUiManager _gameUi;
            private bool _finish;
            private List<BoosterType> _boosters;
            private List<PowerUpData> _registeredPowerUps;

            protected override void OnEnter()
            {
                base.OnEnter();
                _finish = false;
                _gameUi = Agent._gameUi;

                if (_boosters == null)
                {
                    _boosters = new List<BoosterType>();
                    _registeredPowerUps = new List<PowerUpData>();
                }
                else
                {
                    _boosters.Clear();
                    _registeredPowerUps?.Clear();
                }

                if (_gameUi != null)
                {
                    // _gameUi.OpenMach3StartMenu(Agent.boardData.questData);
                    _gameUi.OpenMach3StartMenu(Agent._boardData, Agent._isRandomLevel, Agent._isLuxuryLevel);
                    _gameUi.Match3StartMenuPlayBtnClicked += _gameUi_Match3StartMenuPlayBtnClicked;
                    _gameUi.Match3StartMenu.CloseBtnClicked += Match3StartMenu_CloseBtnClicked;
                    _gameUi.Match3StartMenu.PlayConfirmed += Match3StartMenu_PlayConfirmed;
                    _gameUi.Match3StartMenu.PowerUpTutorialFinished += Match3StartMenu_PowerUpTutorialFinished;
                    _gameUi.Match3StartMenu.ExtraMovesByAdsConfirmed += Match3StartMenu_ExtraMovesByAdsConfirmed;
                    _gameUi.PowerUpSelected += GameUiPowerUpSelected;
                    _gameUi.PowerUpReleased += GameUiPowerUpReleased;
                    _gameUi.NoEnergy.NotEnoughGemsToRefill += NoEnergy_NotEnoughGemsToRefill;
                    _gameUi.NoEnergy.Closed += NoEnergy_Closed;
                    _gameUi.PreMatch3Started += PreMatch3Started;
                    _gameUi.PreMatch3Finished += PreMatch3Finished;
                    _gameUi.PowerUpPurchase.PurchaseBtnClicked += PowerUpPurchase_PurchaseBtnClicked;
                }
                else
                {
                    _finish = true;
                }
            }

            private void Match3StartMenu_ExtraMovesByAdsConfirmed()
            {
                Agent._board.AddMoves(Agent._gameData.MovesToGetByAdsBeforeGame);
                Agent._extraMoves += Agent._gameData.MovesToGetByAdsBeforeGame;
                _gameUi.CloseDarkBg();
                _gameUi.Match3StartMenu.ConfirmPlay();
                // _gameUi?.Match3GameUi.AddMoves(Agent._gameData.MovesToGetByAdsBeforeGame);
            }

            private void Match3StartMenu_PowerUpTutorialFinished()
            {
                ProfileModule pm = Context.Get<ProfileModule>();
                Agent._profile.TutorialStepFinished = 15;
                pm.Save();
            }

            private void NoEnergy_Closed(UiWidget obj)
            {
                _gameUi.Match3StartMenu.ShowBody();
            }

            private void GameUiPowerUpReleased(PowerUpData data)
            {
                // Agent._profile.Wallet.Earn(data.CurrencyName, 1,"PreGameReleased");

                if (Agent._profile.StringVariables.ContainsKey("UsedTutorialPowerup"))
                {
                    CurrencyName currency = CurrencyName.Coin;
                    if (CurrencyName.TryParse(Agent._profile.StringVariables["UsedTutorialPowerup"], out currency))
                    {
                        if (currency == data.CurrencyName)
                        {
                            if (!Agent._profile.StringVariables.ContainsKey("ActiveTutorialPowerup"))
                            {
                                Agent._profile.StringVariables.Add("ActiveTutorialPowerup", Agent._profile.StringVariables["UsedTutorialPowerup"]);
                            }
                            else
                            {
                                Agent._profile.StringVariables["ActiveTutorialPowerup"] =
                                    Agent._profile.StringVariables["UsedTutorialPowerup"];
                            }

                            Agent._profile.StringVariables.Remove("UsedTutorialPowerup");
                        }
                    }
                }

                _boosters.RemoveAll(b => b == data.BoosterType);
                _registeredPowerUps.RemoveAll(p => p == data);
            }

            private void GameUiPowerUpSelected(PowerUpData data)
            {
                // Agent._profile.Wallet.Pay(data.CurrencyName, 1,"PreGameSelect");
                for (int i = 0; i < data.ItemsInPack; i++)
                {
                    _boosters.Add(data.BoosterType);
                }

                _registeredPowerUps.Add(data);
            }

            private void PowerUpPurchase_PurchaseBtnClicked(PowerUpData data, bool inGame)
            {
                string target = data.CurrencyName.ToString();
                if (Agent._profile.StringVariables.ContainsKey("ActiveTutorialPowerup"))
                {
                    CurrencyName currencyName = CurrencyName.Coin;
                    if (CurrencyName.TryParse(Agent._profile.StringVariables["ActiveTutorialPowerup"], out currencyName))
                    {
                        target = "Tutorial";
                    }


                    if (!Agent._profile.StringVariables.ContainsKey("UsedTutorialPowerup"))
                        Agent._profile.StringVariables.Add("UsedTutorialPowerup",
                            Agent._profile.StringVariables["ActiveTutorialPowerup"]);
                    else
                        Agent._profile.StringVariables["UsedTutorialPowerup"] =
                            Agent._profile.StringVariables["ActiveTutorialPowerup"];


                    Agent._profile.StringVariables.Remove("ActiveTutorialPowerup");
                }

                if (Agent._profile.Wallet.Pay(CurrencyName.Gem, data.PriceByGem, target))
                {
                    _gameUi.PowerUpPurchase.Close();
                    //AdjustWrapper.SendAdjustEvent(data.PowerUpBoughtAdjustEvent);
                    GameAnalytics.NewDesignEvent("Monitization_PowerUps_Double" + data.BoosterType + "_Gained");
                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, CurrencyName.Gem.ToString(), data.PriceByGem, "PreGameBooster", "Double_" + data.BoosterType.ToString());
                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, data.CurrencyName.ToString(), (uint) (data.NumberOfPacks), "PreGameBooster", "Double_" + data.BoosterType.ToString());
                    if (!target.Equals("Tutorial"))
                        target = "Direct";
                    Agent._profile.Wallet.Earn(data.CurrencyName, (uint) (data.NumberOfPacks), target);
                    _gameUi.Match3StartMenu.SelectPowerUp(data);
                    for (int i = 0; i < data.ItemsInPack; i++)
                    {
                        _boosters.Add(data.BoosterType);
                    }

                    _registeredPowerUps.Add(data);
                }
                else
                {
                    if (inGame)
                    {
                        GameAnalytics.NewDesignEvent(Strings.Monetization + ":InBooster:" + Strings.PurchaseInBoosterButtonPressed);

                    }
                    else
                    {
                        GameAnalytics.NewDesignEvent(Strings.Monetization + ":PreBooster:" + Strings.PurchasePreBoosterButtonPressed);

                    }


                    if (inGame)
                    {
                        if (!((PlayerProfileV3) Agent._profile).StringVariables.ContainsKey(Strings.CurrentShopSource))
                            ((PlayerProfileV3) Agent._profile).StringVariables.Add(Strings.CurrentShopSource, Strings.Monetization + ":InBooster:" + Strings.InBoosterPurchaseSuccess);
                        ((PlayerProfileV3) Agent._profile).StringVariables[Strings.CurrentShopSource] =
                            Strings.Monetization + ":InBooster:" + Strings.InBoosterPurchaseSuccess;
                        _gameUi.OpenShopMenu(Strings.Monetization + ":InBooster:" + Strings.InBoosterShopWindowShown);
                    }
                    else
                    {
                        if (!((PlayerProfileV3) Agent._profile).StringVariables.ContainsKey(Strings.CurrentShopSource))
                            ((PlayerProfileV3) Agent._profile).StringVariables.Add(Strings.CurrentShopSource, Strings.Monetization + ":PreBooster:" + Strings.PreBoosterPurchaseSuccess);
                        ((PlayerProfileV3) Agent._profile).StringVariables[Strings.CurrentShopSource] =
                            Strings.Monetization + ":PreBooster:" + Strings.PreBoosterPurchaseSuccess;
                        _gameUi.OpenShopMenu(Strings.Monetization + ":PreBooster:" + Strings.PreBoosterShopWindowShown);
                    }


                    //AdjustWrapper.SendAdjustEvent(Strings.Match3NotEnoughGemsForPowerUpPreGameShopOpenAdjustEvent);
                }
            }

            private void Match3StartMenu_PlayConfirmed()
            {
                var profv3 = (PlayerProfileV3) Agent._profileModule.GetLatestActiveProfile();
                Agent._board.NumberOFPreGameBoosterUsed = 0;
                Agent._board.NumberOfTimesMovePurchased = 0;
                Agent._board.NumberOfInGameBoosterUsed = 0;
                if (!profv3.IntVariables.ContainsKey("LastMatch3LevelStarted"))
                {
                    GameAnalytics.NewDesignEvent("Match3:FirstStart:Playtime:" + Agent._boardData.name,Agent._profile.IntVariables[Strings.TotalSessionsLengthMinute]);
                    GameAnalytics.NewDesignEvent("Match3:FirstStart:Gem:" + Agent._boardData.name, Agent._profile.Wallet.GetAmount(CurrencyName.Gem));
                    GameAnalytics.NewDesignEvent("Match3:FirstStart:Coin:" + Agent._boardData.name, Agent._profile.Wallet.GetAmount(CurrencyName.Coin));
                    GameAnalytics.NewDesignEvent("Match3:FirstStart:Energy:" + Agent._boardData.name, Agent._profile.Wallet.GetAmount(CurrencyName.Energy));
                    profv3.IntVariables.Add("LastMatch3LevelStarted", Agent._boardData.index);
                }
                else if (profv3.IntVariables["LastMatch3LevelStarted"] != Agent._boardData.index)
                {
                    GameAnalytics.NewDesignEvent("Match3:FirstStart:Playtime:" + Agent._boardData.name, Agent._profile.IntVariables[Strings.TotalSessionsLengthMinute]);
                    GameAnalytics.NewDesignEvent("Match3:FirstStart:Gem:" + Agent._boardData.name, Agent._profile.Wallet.GetAmount(CurrencyName.Gem));
                    GameAnalytics.NewDesignEvent("Match3:FirstStart:Coin:" + Agent._boardData.name, Agent._profile.Wallet.GetAmount(CurrencyName.Coin));
                    GameAnalytics.NewDesignEvent("Match3:FirstStart:Energy:" + Agent._boardData.name, Agent._profile.Wallet.GetAmount(CurrencyName.Energy));
                    profv3.IntVariables["LastMatch3LevelStarted"] = Agent._boardData.index;
                }

                foreach (var powerUp in _registeredPowerUps)
                {
                    var currency = CurrencyName.Coin;

                    if (Agent._profile.StringVariables.ContainsKey("UsedTutorialPowerup"))
                    {
                        if (CurrencyName.TryParse(Agent._profile.StringVariables["UsedTutorialPowerup"], out currency))
                        {
                            if (currency == powerUp.CurrencyName)
                            {
                                Agent._profile.Wallet.Pay(powerUp.CurrencyName, 1, "Tutorial");
                                Agent._profile.StringVariables.Remove("UsedTutorialPowerup");
                                continue;
                            }
                        }
                    }

                    Agent._profile.Wallet.Pay(powerUp.CurrencyName, 1, "PreGameSelect");
                }

                int movesToBeAddedForSessionCount = 0;

                if (Agent._profile.IntVariables.ContainsKey(Strings.TotalSessionCount))
                {
                    if (Agent._profile.IntVariables.ContainsKey(Strings.CurrentMatch3Retries))
                        if (Agent._profile.IntVariables[Strings.CurrentMatch3Retries] >
                            Agent._boardData.criticalSessionCount && Agent._boardData.criticalSessionCount > 0)
                        {
                            movesToBeAddedForSessionCount =
                                Agent._boardData.criticalFinalMoves - Agent._boardData.questData.totalMoves;
                        }
                }

                if (movesToBeAddedForSessionCount > 0)
                {
                    Agent._board.AddMoves(movesToBeAddedForSessionCount);
                    Agent._extraMoves += movesToBeAddedForSessionCount;
                }

                _gameUi.StartMatch3Game(Agent._boardData.questData);

                //AdjustWrapper.SendAdjustEvent(Strings.Match3LevelStartAdjustEvent);
                //AdjustWrapper.SendAdjustEvent(Agent._boardData.AdjustLevelStartEvent);
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, Agent._boardData.name);

                if (Agent._boardData.index == 1)
                {
                    GameAnalytics.NewDesignEvent("T21_Level1Match3Level1_Start");
                }
                else if (Agent._boardData.index == 2)
                {
                    GameAnalytics.NewDesignEvent("T31_Level1Match3Level2_Start");
                }
            }

            private void NoEnergy_NotEnoughGemsToRefill()
            {
                //AdjustWrapper.SendAdjustEvent(Strings.Match3EnergyRefillShopOpenAdjustEvent);
                if (!((PlayerProfileV3) Agent._profile).StringVariables.ContainsKey(Strings.CurrentShopSource))
                    ((PlayerProfileV3) Agent._profile).StringVariables.Add(Strings.CurrentShopSource, Strings.Monetization + ":Energy:" + Strings.EnergyPurchaseSuccess);
                ((PlayerProfileV3) Agent._profile).StringVariables[Strings.CurrentShopSource] = Strings.Monetization + ":Energy:" + Strings.EnergyPurchaseSuccess;


                _gameUi.OpenShopMenu(Strings.Monetization + ":Energy:" + Strings.EnergyShopWindowShown);
            }

            private void _gameUi_Match3StartMenuPlayBtnClicked()
            {
                if (Agent._profile.Wallet.GetAmount(CurrencyName.Energy) < 1)
                {
                    if (Agent._isLuxuryLevel)
                    {
                        _gameUi.CloseDarkBg();
                        _gameUi.Match3StartMenu.ConfirmPlay();
                    }
                    else
                    {
                        GameAnalytics.NewDesignEvent(Strings.Monetization + ":Energy:" + Strings.OutOfEnergyWindowShown);


                        var energyModule = Context.Get<EnergyModule>();

                        _gameUi.NoEnergy.SetData(Agent._profile.Wallet, Agent._gameData.EnergyRefillPrice, energyModule, Agent._profileModule);

                        _gameUi.Match3StartMenu.HideBody(() => { _gameUi.OpenNoEnergy(); });
                    }
                }
                else
                {
                    _gameUi.CloseDarkBg();
                    _gameUi.Match3StartMenu.ConfirmPlay();
                }
            }

            private void Match3StartMenu_CloseBtnClicked()
            {
                foreach (var powerUp in _registeredPowerUps)
                {
                    // GameUiPowerUpReleased(powerUp);
                    _gameUi.Match3StartMenu.UnSelectPowerUp(powerUp);
                }

                _registeredPowerUps.Clear();
                _boosters.Clear();
                Agent.ExitWithoutPlay = true;
                _finish = true;
                //AdjustWrapper.SendAdjustEvent(Agent._boardData.AdjustLevelQuitEvent);
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, Agent._boardData.name);
                GameAnalytics.NewDesignEvent(Agent._boardData.name + "_QuitConfirmed");
            }

            private void PreMatch3Started()
            {
                // Agent._profile.Wallet.Print();
                var preGameBoosterUsed = false;
                Agent._board.NumberOFPreGameBoosterUsed = _registeredPowerUps.Count;
                Agent.CheckForAds?.Invoke();
                foreach (var powerUp in _registeredPowerUps)
                {
                    GameAnalytics.NewDesignEvent("Monitization_Double_" + powerUp.BoosterType + "_Used");

                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, powerUp.CurrencyName.ToString(), (uint) (powerUp.NumberOfPacks), "PreGameBooster", "Double_" + powerUp.BoosterType.ToString());

                    //AdjustWrapper.SendAdjustEvent(powerUp.PowerUpUsedAdjustEvent);
                    preGameBoosterUsed = true;
                }

                if (preGameBoosterUsed)
                {
                    //AdjustWrapper.SendAdjustEvent(Agent._boardData.AdjustPreGameBoosterUsedEvent);
                    GameAnalytics.NewDesignEvent(Agent._boardData.name + "_BeforeEnterBoosterUsed");
                }

                Agent.SetBoardBoosters(_boosters);
            }

            private void PreMatch3Finished()
            {
                Agent.ExitWithoutPlay = false;
                _finish = true;
            }

            protected override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                if (_finish)
                {
                    Finished();
                }
            }

            protected override void OnExit()
            {
                base.OnExit();
                if (_gameUi == null) return;
                _gameUi.PreMatch3Started -= PreMatch3Started;
                _gameUi.PreMatch3Finished -= PreMatch3Finished;
                _gameUi.Match3StartMenu.CloseBtnClicked -= Match3StartMenu_CloseBtnClicked;
                _gameUi.Match3StartMenuPlayBtnClicked -= _gameUi_Match3StartMenuPlayBtnClicked;
                _gameUi.Match3StartMenu.PowerUpTutorialFinished -= Match3StartMenu_PowerUpTutorialFinished;
                _gameUi.Match3StartMenu.PlayConfirmed -= Match3StartMenu_PlayConfirmed;
                _gameUi.Match3StartMenu.ExtraMovesByAdsConfirmed -= Match3StartMenu_ExtraMovesByAdsConfirmed;

                _gameUi.PowerUpSelected -= GameUiPowerUpSelected;
                _gameUi.PowerUpReleased -= GameUiPowerUpReleased;
                _gameUi.NoEnergy.NotEnoughGemsToRefill -= NoEnergy_NotEnoughGemsToRefill;
                _gameUi.PowerUpPurchase.PurchaseBtnClicked -= PowerUpPurchase_PurchaseBtnClicked;
            }
        }

        public class PlayingState : PuzzleState
        {
            private GameUiManager _gameUi;
            private GameData _gameData;
            private PlayerProfileV3 _profile;
            private AudioPlayer _audioPlayer;
            private bool _finish;
            private int _moveCount;
            private List<BoosterType> _boosters;
            private PowerUpData _registeredPowerUp;
            private bool _usedInGameBooster = false;
            private bool _initialReduction;


            protected override void OnEnter()
            {
                _usedInGameBooster = false;
                _finish = false;
                _moveCount = 0;
                _gameUi = Agent._gameUi;
                _gameData = Agent._gameData;
                _profile = Agent._profile;
                _audioPlayer = Agent._audioPlayer;

                if (Context != null)
                {
                    UiModule ui = Context.Get<UiModule>();
                    Match3GameUi.match3Camera = GameObject.FindWithTag("match3_camera").GetComponent<Camera>();
                    ui.CanvasRoot.worldCamera = Match3GameUi.match3Camera;
                    ui.CanvasRoot.renderMode = RenderMode.ScreenSpaceCamera;
                    ui.CanvasRoot.planeDistance = Match3GameUi.match3Camera.nearClipPlane + 0.2f;
                    ui.CanvasRoot.sortingOrder = 1000;
                }

                base.OnEnter();
                ClickableObjectSystem.Instance.SetCamera(Agent._puzzleCamera, true, true);
                GridClickDetector.SetCamera(Agent._puzzleCamera);


                if (_gameUi != null)
                {
                    //     _gameUi.CloseMatch3Game += CloseMatch3Game;
                    _gameUi.OpenMatch3Game(Agent._boardData, Agent._isRandomLevel, Agent._isLuxuryLevel, Agent._extraMoves);

                    _gameUi.PauseMenuExitBtnClicked += PauseMenuExitBtnClicked;
                    _gameUi.Match3OutOfMovesConfirmed += _gameUi_Match3OutOfMovesConfirmed;
                    _gameUi.PurchaseExtraMovesByGem += GameUiPurchaseExtraMovesByGem;
                    _gameUi.Match3OutOfMoves.ExtraMovesByAdsConfirmed += Match3OutOfMovesExtraMovesByAds;
                    _gameUi.Match3OutOfMoves.ExtraMoveFromPackConfirmed += Match3OutOfMoves_ExtraMoveFromPackConfirmed;
                    _gameUi.EnergyReduction.CloseBtnClicked += EnergyReduction_CloseBtnClicked;

                    _gameUi.PowerUpPurchase.PurchaseBtnClicked += PowerUpPurchase_PurchaseBtnClicked;

                    _gameUi.PowerUpSelected += GameUiPowerUpSelected;
                    _gameUi.PowerUpReleased += GameUiPowerUpReleased;
                    Agent._extraMoves = 0;
                }

                var cam = Camera.main;
                if (cam != null) cam.enabled = false;
                QuestGiver.OnFinished += _board_OnFinished;
                QuestGiver.OnBlocksMatched += _board_OnBlocksMatched;
                Agent._board.OnFingerBoosterUsed += _board_OnFingerBoosterUsed;
            }

            private void Match3OutOfMoves_ExtraMoveFromPackConfirmed()
            {
                Agent._board.AddMoves(_gameData.MoveToBeAddedOnPurchaseWithGem);
                _gameUi.CloseDarkBg();
                _gameUi?.Match3OutOfMoves.Close();
                _gameUi?.Match3GameUi.AddMoves(_gameData.MoveToBeAddedOnPurchaseWithGem);
                _audioPlayer.PlayClip("Match3Music");
            }

            private void Match3OutOfMovesExtraMovesByAds()
            {
                Agent._board.AddMoves(_gameData.MoveToBeAddedByAds);
                _gameUi.CloseDarkBg();
                _gameUi?.Match3OutOfMoves.Close();
                _gameUi?.Match3GameUi.AddMoves(_gameData.MoveToBeAddedByAds);
                _audioPlayer.PlayClip("Match3Music");
                _gameUi.Match3OutOfMoves.VideoAvailable(false, _profile.State.LastMatch3LevelWon);
            }

            private void _board_OnFingerBoosterUsed()
            {
                string target = "InGameUse";
                if (_profile != null && _profile.StringVariables != null && _profile.StringVariables.ContainsKey("UsedTutorialPowerup"))
                {
                    CurrencyName currencyName = CurrencyName.Coin;
                    if (CurrencyName.TryParse(_profile.StringVariables["UsedTutorialPowerup"], out currencyName))
                    {
                        if (currencyName == _registeredPowerUp.CurrencyName)
                            target = "Tutorial";
                    }
                }


                //Log.Debug("Match3", $"Finger Booster used in puzzlecontroller");
                if (_registeredPowerUp == null) return;
                if (Agent._profile.Wallet.Pay(_registeredPowerUp.CurrencyName, 1, target) && target.Equals("Tutorial"))
                    _profile.StringVariables.Remove("UsedTutorialPowerup");
                GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, _registeredPowerUp.CurrencyName.ToString(), (uint) (_registeredPowerUp.NumberOfPacks), "InGameBooster", _registeredPowerUp.BoosterType.ToString());
                _gameUi?.Match3GameUi.UsePowerUp(_registeredPowerUp);
                _registeredPowerUp = null;
            }

            private void PowerUpPurchase_PurchaseBtnClicked(PowerUpData data, bool inGame)
            {
                string target = data.CurrencyName.ToString();
                if (_profile.StringVariables.ContainsKey("ActiveTutorialPowerup"))
                {
                    CurrencyName currencyName = CurrencyName.Coin;
                    if (CurrencyName.TryParse(_profile.StringVariables["ActiveTutorialPowerup"], out currencyName))
                    {
                        target = "Tutorial";
                    }
                }


                if (Agent._profile.Wallet.Pay(CurrencyName.Gem, data.PriceByGem, target))
                {
                    if (_profile.StringVariables.ContainsKey("ActiveTutorialPowerup"))
                    {
                        if (_profile.StringVariables.ContainsKey("UsedTutorialPowerup"))
                        {
                            _profile.StringVariables["UsedTutorialPowerup"] =
                                _profile.StringVariables["ActiveTutorialPowerup"];
                        }
                        else
                        {
                            _profile.StringVariables.Add("UsedTutorialPowerup", _profile.StringVariables["ActiveTutorialPowerup"]);
                        }
                    }

                    _profile.StringVariables.Remove("ActiveTutorialPowerup");
                    Agent._board.NumberOfInGameBoosterUsed++;
                    _gameUi.PowerUpPurchase.Close();
                    _gameUi.CloseDarkBg();
                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, CurrencyName.Gem.ToString(), data.PriceByGem, "InGameBooster", data.BoosterType.ToString());
                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, data.CurrencyName.ToString(), (uint) (data.NumberOfPacks), "InGameBooster", data.BoosterType.ToString());

                    if (!target.Equals("Tutorial"))
                        target = "Direct";

                    Agent._profile.Wallet.Earn(data.CurrencyName, (uint) (data.NumberOfPacks), target);
                    _gameUi.Match3GameUi.SelectPowerUp(data);
                    ClearRegisteredPowerUp();
                    Agent.UnSetBoardFingerBooster();
                    _registeredPowerUp = data;
                    Agent.SetBoardFingerBooster(data.BoosterType);
                    GameAnalytics.NewDesignEvent("Monitization_Powerups_" + data.BoosterType + "_Gained");
                    if (_usedInGameBooster)
                    {
                        GameAnalytics.NewDesignEvent(Agent._boardData.name + "_InGameBoosterUsed");
                        _usedInGameBooster = true;
                    }
                }
                else
                {
                    if (inGame)
                    {
                        if (!((PlayerProfileV3) _profile).StringVariables.ContainsKey(Strings.CurrentShopSource))
                            ((PlayerProfileV3) Agent._profile).StringVariables.Add(Strings.CurrentShopSource,
                                Strings.Monetization + ":InBooster:" + Strings.PreBoosterPurchaseSuccess);
                        ((PlayerProfileV3) _profile).StringVariables[Strings.CurrentShopSource] = Strings.Monetization + ":InBooster:" + Strings.InBoosterPurchaseSuccess;
                    }
                    else
                    {
                        if (!((PlayerProfileV3) Agent._profile).StringVariables.ContainsKey(Strings.CurrentShopSource))
                            ((PlayerProfileV3) Agent._profile).StringVariables.Add(Strings.CurrentShopSource, Strings.Monetization + ":PreBooster:" + Strings.PreBoosterPurchaseSuccess);
                        ((PlayerProfileV3) Agent._profile).StringVariables[Strings.CurrentShopSource] =
                            Strings.Monetization + ":PreBooster:" + Strings.PreBoosterPurchaseSuccess;
                    }

                    if (inGame)
                    {
                        GameAnalytics.NewDesignEvent(Strings.Monetization + ":InBooster:" + Strings.PurchaseInBoosterButtonPressed);

                        _gameUi.OpenShopMenu(Strings.Monetization + ":InBooster:" + Strings.InBoosterShopWindowShown);
                    }
                    else
                    {
                        GameAnalytics.NewDesignEvent(Strings.Monetization + ":PreBooster:" + Strings.PurchasePreBoosterButtonPressed);

                        _gameUi.OpenShopMenu(Strings.Monetization + ":PreBooster:" + Strings.PreBoosterShopWindowShown);
                    }
                }
            }

            private void ClearRegisteredPowerUp()
            {
                if (_registeredPowerUp)
                    _gameUi.Match3GameUi.UnSelectPowerUp(_registeredPowerUp);
                _registeredPowerUp = null;
            }

            private void GameUiPowerUpReleased(PowerUpData data)
            {
                _registeredPowerUp = null;
                // Agent._profile.Wallet.Earn(data.CurrencyName, 1);
                Agent.UnSetBoardFingerBooster();
            }

            private void GameUiPowerUpSelected(PowerUpData data)
            {
                ClearRegisteredPowerUp();
                Agent.UnSetBoardFingerBooster();
                _registeredPowerUp = data;
                //Agent._profile.Wallet.Pay(data.CurrencyName, 1);
                Agent.SetBoardFingerBooster(data.BoosterType);
            }

            private void GameUiPurchaseExtraMovesByGem()
            {
                var isPayed = _profile.Wallet.Pay(CurrencyName.Gem, (uint) _gameData.ExtraMovesPriceByGem, "MoreMove", Agent._boardData.index);
                if (isPayed)
                {
                    //AdjustWrapper.SendAdjustEvent(Agent._boardData.AdjustMovePurchaseEvent);

                    GameAnalytics.NewDesignEvent("Match3:MovePurchased:" + Agent._boardData.name);


                    GameAnalytics.NewResourceEvent(GAResourceFlowType.Sink, CurrencyName.Gem.ToString(), (uint) _gameData.ExtraMovesPriceByGem, "Move", "ExtraMove");
                    Agent._board.AddMoves(_gameData.MoveToBeAddedOnPurchaseWithGem);
                    _gameUi.CloseDarkBg();
                    _gameUi?.Match3OutOfMoves.Close();
                    _gameUi.PopTopMostWindow();
                    _gameUi?.Match3GameUi.AddMoves(_gameData.MoveToBeAddedOnPurchaseWithGem);
                    Agent._board.NumberOfTimesMovePurchased++;
                    _audioPlayer.PlayClip("Match3Music");
                }
                else
                {
                    GameAnalytics.NewDesignEvent(Strings.Monetization + ":Move:" + Strings.PurchaseMoveButtonPressed);


                    if (!((PlayerProfileV3) Agent._profile).StringVariables.ContainsKey(Strings.CurrentShopSource))
                        ((PlayerProfileV3) Agent._profile).StringVariables.Add(Strings.CurrentShopSource, Strings.Monetization + ":Move:" + Strings.MovePurchaseSuccess);
                    ((PlayerProfileV3) Agent._profile).StringVariables[Strings.CurrentShopSource] = Strings.Monetization + ":Move:" + Strings.MovePurchaseSuccess;

                    //AdjustWrapper.SendAdjustEvent(Strings.Match3MoreMoveShopOpenAdjustEvent);
                    _gameUi.OpenShopMenu(Strings.Monetization + ":Move:" + Strings.MoveShopWindowShown);
                }
            }

            private void _board_OnBlocksMatched(MoveData moveData)
            {
                _moveCount++;
                if (_moveCount == 1)
                {
                    _profile?.Wallet?.Pay(CurrencyName.Energy, 1, "Match3Level");
                    _initialReduction = true;
                }

                _gameUi?.Match3GameUi.UpdateOnMatches(moveData);
            }

            private void _board_OnFinished(Match3Result result)
            {
                stateContext = new ResultStateContext
                {
                    PlayResult = result
                };

                if (_gameUi == null) return;
                if (result.winStatus == WinStatus.Win)
                {
                    _profile.Wallet.Earn(CurrencyName.Energy, 1, "Match3LevelWin");
                    _audioPlayer.PlayClip("Match3WinTheme", 0.2f);
                    _finish = true;


                    var activeEvent = _profile.ProfileEventDatas.FirstOrDefault(eventData =>
                        !eventData.IsExpired && !eventData.IsFinished && eventData.CurrentMatch3LvAtEnter > 0);
                    if (activeEvent != null)
                    {
                        activeEvent.NumberOfMatch3LevelsWon++;
                        if (Agent._isRandomLevel)
                            activeEvent.NumberOfMatch3RandomLevelsWon++;
                        activeEvent.IsRandomLevel = Agent._isRandomLevel;
                    }


                    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, Agent._boardData.name);
                    //Debug.Log("" + Agent._boardData.name);

                    //normal win
                    GameAnalytics.NewDesignEvent("Match3:Win:Win:" + Agent._boardData.name);


                    //moves left
                    GameAnalytics.NewDesignEvent("Match3:Win:MoveLeft:" + Agent._boardData.name, QuestGiver.CurrentMoves);


                    //number of time move purchased
                    GameAnalytics.NewDesignEvent("Match3:Win:NumberOfMovePurchased:" + Agent._boardData.name, Agent._board.NumberOfTimesMovePurchased);


                    //number of time ingame booster purchased
                    GameAnalytics.NewDesignEvent("Match3:Win:NumberOfInGameBoosterUsed:" + Agent._boardData.name, Agent._board.NumberOfInGameBoosterUsed);


                    //number of time pregame booster purchased
                    GameAnalytics.NewDesignEvent("Match3:Win:NumberOFPreGameBoosterUsed:" + Agent._boardData.name, Agent._board.NumberOFPreGameBoosterUsed);


                    //
                    GameAnalytics.NewDesignEvent("Match3:Win:Energy:" + Agent._boardData.name, Agent._profile.Wallet.GetAmount(CurrencyName.Energy));
                    GameAnalytics.NewDesignEvent("Match3:Win:Gem:" + Agent._boardData.name, Agent._profile.Wallet.GetAmount(CurrencyName.Gem));

                    //

                    GameAnalytics.NewDesignEvent("Design_Match3level_Finish");
                    if (Agent._boardData.index == 1)
                    {
                        GameAnalytics.NewDesignEvent("T23_Level1Match3Level1_Finished");
                    }
                    else if (Agent._boardData.index == 2)
                    {
                        GameAnalytics.NewDesignEvent("T36_Level1Match3Level2_Finished");
                    }
                }
                else
                {
                    _audioPlayer.PlayClip("Match3LoseTheme");
                    _gameUi.OpenDarkBg(0);
                    _gameUi.OpenMatch3OutOfMoves();
                    _gameUi.Match3OutOfMoves.Init(result,
                        _gameData,
                        Agent._boardData, (PlayerProfileV2) _profile);
                    Debug.LogError("should only show once");
                    GameAnalytics.NewDesignEvent(Strings.Monetization + ":Move:" + Strings.OutOfMoveWindowShown);

                    GameAnalytics.NewDesignEvent(Agent._boardData.name + "_NoMoveWindowShown");
                }
            }

            private void PauseMenuExitBtnClicked()
            {
                if (Agent._isLuxuryLevel)
                {
                    if (_initialReduction)
                    {
                        _profile.Wallet.Earn(CurrencyName.Energy, 1, "Match3Level");
                        _initialReduction = false;
                    }

                    Match3FinishConfirmed();
                    return;
                }


                if (_moveCount > 0 && _profile.State.InfiniteEnergyEndTime <= GameTime.Now())
                {
                    _gameUi.OpenExitConfirmation();
                    return;
                }
                else
                {
                    if (_initialReduction)
                    {
                        _profile.Wallet.Earn(CurrencyName.Energy, 1, "Match3Level");
                        _initialReduction = false;
                    }

                    Match3FinishConfirmed();
                }
            }

            private void Match3FinishConfirmed()
            {
                Agent.DisposeBoard();
                _gameUi.ConfirmCloseMatch3Game();
                if (Agent._isLuxuryLevel)
                {
                    _gameUi.LuxurySourceUi.Match3Result(_moveCount > 0, false);
                }

                Agent._profileModule.Save();
                _finish = true;
            }

            private void _gameUi_Match3OutOfMovesConfirmed()
            {
                //AdjustWrapper.SendAdjustEvent(Agent._boardData.AdjustLevelLoseEvent);
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, Agent._boardData.name);

                GameAnalytics.NewDesignEvent("Design_" + Agent._boardData.name + "_Defeat");
                GameAnalytics.NewDesignEvent("Design_Match3Level_Lose");

                //AdjustWrapper.SendAdjustEvent(Strings.Match3LevelFinishAdjustEvent);
                GameAnalytics.NewDesignEvent("Match3:Defeat:" + Agent._boardData.name);


                GameAnalytics.NewDesignEvent("Match3:Defeat:NumberOfMovePurchased:" + Agent._boardData.name, Agent._board.NumberOfTimesMovePurchased);


                //number of time ingame booster purchased
                GameAnalytics.NewDesignEvent("Match3:Defeat:NumberOfInGameBoosterUsed:" + Agent._boardData.name, Agent._board.NumberOfInGameBoosterUsed);


                //number of time pregame booster purchased
                GameAnalytics.NewDesignEvent("Match3:Defeat:NumberOFPreGameBoosterUsed:" + Agent._boardData.name, Agent._board.NumberOFPreGameBoosterUsed);


                var profileV3 = (PlayerProfileV3) Agent._profile;
                if (profileV3 != null)
                {
                    if (profileV3.IntVariables == null)
                        profileV3.IntVariables = new Dictionary<string, int>();
                    if (!profileV3.IntVariables.ContainsKey(Strings.NumberOfMatch3Retries))
                        profileV3.IntVariables.Add(Strings.NumberOfMatch3Retries, 1);
                    else
                        profileV3.IntVariables[Strings.NumberOfMatch3Retries]++;


                    if (!_profile.IntVariables.ContainsKey(Strings.LastMatch3LevelPlayed))
                    {
                        _profile.IntVariables.Add(Strings.LastMatch3LevelPlayed, _profile.State.LastMatch3LevelWon + 1);
                        if (!_profile.IntVariables.ContainsKey(Strings.CurrentMatch3Retries))
                            _profile.IntVariables.Add(Strings.CurrentMatch3Retries, 1);
                        else
                        {
                            _profile.IntVariables[Strings.CurrentMatch3Retries] = 1;
                        }
                    }
                    else
                    {
                        if (_profile.IntVariables[Strings.LastMatch3LevelPlayed] == _profile.State.LastMatch3LevelWon + 1)
                        {
                            _profile.IntVariables[Strings.CurrentMatch3Retries]++;
                        }
                        else
                        {
                            _profile.IntVariables[Strings.CurrentMatch3Retries] = 1;
                            _profile.IntVariables[Strings.LastMatch3LevelPlayed] = _profile.State.LastMatch3LevelWon + 1;
                        }
                    }

                    Agent._profileModule.Save();
                }

                _gameUi.CloseDarkBg();
                if (Agent._isLuxuryLevel)
                {
                    Match3FinishConfirmed();
                }
                else
                {
                    _gameUi.EnergyReduction.Open();
                }
            }

            private void EnergyReduction_CloseBtnClicked()
            {
                if (QuestGiver.CurrentMoves > 0)
                {
                    GameAnalytics.NewDesignEvent("Match3:Quit:Quit:" + Agent._boardData.name);


                    GameAnalytics.NewDesignEvent("Match3:Quit:MoveLeft:" + Agent._boardData.name, QuestGiver.CurrentMoves);


                    if (Agent._profile.Wallet.GetAmount(CurrencyName.Energy) < 1)
                    {
                        if (!Agent._profile.IntVariables.ContainsKey(Strings.NumberOfTimesEnergyDepleted))
                            Agent._profile.IntVariables.Add(Strings.NumberOfTimesEnergyDepleted, 0);
                        Agent._profile.IntVariables[Strings.NumberOfTimesEnergyDepleted]++;

                        var prof = Agent._profile;

                        if (prof != null)
                        {
                            var completed = 0;
                            if (prof.State.StageStates.ContainsKey(prof.State.LatestLoadedLevel))
                            {
                                foreach (var actionState in prof.State.StageStates[prof.State.LatestLoadedLevel].ActionStates)
                                {
                                    if (actionState.Value.IsPurchased)
                                        completed++;
                                }

                                GameAnalytics.NewDesignEvent("Sink:Energy:DepletedDesign:" + prof.State.LatestLoadedLevel + ":" + completed);

                            }

                            var level = (prof.State.LastMatch3LevelWon + 1).ToString();
                            if ((prof.State.LastMatch3LevelWon + 1) < 1000 && (prof.State.LastMatch3LevelWon + 1) >= 100)
                                level = "0" + level;
                            else if ((prof.State.LastMatch3LevelWon + 1) >= 10 && (prof.State.LastMatch3LevelWon + 1) < 1000)
                                level = "00" + level;
                            else if ((prof.State.LastMatch3LevelWon + 1) < 1000)
                                level = "000" + level;
                            GameAnalytics.NewDesignEvent("Sink:Energy:Depleted:Match3Level_" + level);

                            GameAnalytics.NewDesignEvent("Design_Window_RefillEnergy_Shown");
                        }
                    }
                }

                Match3FinishConfirmed();
            }

            protected override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);
                if (_finish) Finished();
                if (Agent._board == null) return;
                Agent._board.Update(deltaTime);
            }

            protected override void OnExit()
            {
                base.OnExit();

                QuestGiver.OnFinished -= _board_OnFinished;
                QuestGiver.OnBlocksMatched += _board_OnBlocksMatched;
                Agent._board.OnFingerBoosterUsed += _board_OnFingerBoosterUsed;

                if (_gameUi != null)
                {
                    _gameUi.PauseMenuExitBtnClicked -= PauseMenuExitBtnClicked;
                    _gameUi.PurchaseExtraMovesByGem -= GameUiPurchaseExtraMovesByGem;
                    _gameUi.Match3OutOfMoves.ExtraMovesByAdsConfirmed -= Match3OutOfMovesExtraMovesByAds;
                    _gameUi.Match3OutOfMovesConfirmed -= _gameUi_Match3OutOfMovesConfirmed;
                    _gameUi.EnergyReduction.CloseBtnClicked -= EnergyReduction_CloseBtnClicked;
                    _gameUi.PowerUpPurchase.PurchaseBtnClicked -= PowerUpPurchase_PurchaseBtnClicked;
                    _gameUi.Match3OutOfMoves.ExtraMoveFromPackConfirmed -= Match3OutOfMoves_ExtraMoveFromPackConfirmed;
                    _gameUi.PowerUpSelected -= GameUiPowerUpSelected;
                    _gameUi.PowerUpReleased -= GameUiPowerUpReleased;

                    UiModule ui = Context.Get<UiModule>();
                    ui.CanvasRoot.worldCamera = Match3GameUi.match3Camera;
                    ui.CanvasRoot.renderMode = RenderMode.ScreenSpaceOverlay;

                    ui.CanvasRoot.sortingOrder = 0;
                }
            }
        }

        public class ExitState : PuzzleState
        {
            private GameUiManager _gameUi;
            private PlayerProfile _profile;
            private GameData _gameData;
            private ResultStateContext _resultContext;
            private bool _finish;

            protected override void OnEnter()
            {
                base.OnEnter();
                _finish = false;
                _gameData = Agent._gameData;
                _gameUi = Agent._gameUi;
                _resultContext = (ResultStateContext) stateContext;
                _gameUi.Match3WinNextButtonClicked += _gameUi_Match3WinNextButtonClicked;
                _profile = Agent._profile;

                if (Agent.ExitWithoutPlay)
                {
                    _finish = true;
                    return;
                }

                Camera mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                mainCam.enabled = true;
                ClickableObjectSystem.Instance.SetCamera(mainCam, true, true);

                if (_resultContext?.PlayResult.winStatus == WinStatus.Win)
                {
                    _gameUi.PopTopMostWindow();

                    uint coinsWon = Agent._isLuxuryLevel ? 0 : CalcReward((uint) _resultContext.PlayResult.RemainingMoves);
                    uint boardReward = Agent._isLuxuryLevel ? 0 : Agent._boardData.reward;
                    uint couponsWon = Agent._isLuxuryLevel ? 0 : (uint) _resultContext.PlayResult.TotalCoupons;
                    int levelIndex = Agent._isLuxuryLevel ? Agent._boardData.index : 0;
                    _gameUi.Match3Win.SetData(coinsWon, boardReward, couponsWon, levelIndex, Agent._audioPlayer, Agent._isRandomLevel, _gameData.GemsToGetByAdsAfterWinMatch3, (PlayerProfileV3) _profile);
                    _gameUi.Match3Win.Open();
                }
                else
                {
                    _gameUi.UiBackground.Close();
                    _gameUi.Match3GameUi.Close();
                    _finish = true;
                }
            }

            private void _gameUi_Match3WinNextButtonClicked()
            {
                Agent.DisposeBoard();
                _finish = true;
            }

            protected override void OnUpdate(float deltaTime)
            {
                base.OnUpdate(deltaTime);

                if (Agent.ExitWithoutPlay)
                {
                    Finished();
                    return;
                }

                if (!_finish) return;

                if (_resultContext != null && _resultContext.PlayResult.winStatus == WinStatus.Win)
                {
                    uint coins = CalcReward((uint) _resultContext.PlayResult.RemainingMoves);
                    Agent.WinTheGame(coins, (uint) _resultContext.PlayResult.TotalCoupons);
                }
                else
                {
                    Agent.Restart = true;
                    if (Agent._isLuxuryLevel)
                    {
                        Agent.Restart = false;
                    }
                }

                Finished();
            }

            private uint CalcBonusCoin(uint moveLeft)
            {
                uint levelBonus = (uint) (Agent._boardData.reward * _gameData.LevelBonusRatio);
                float percent = (float) moveLeft / Agent._boardData.questData.totalMoves;
                uint bonusAmount = (uint) (percent * levelBonus);
                uint maxBonus = (uint) (Agent._boardData.reward * _gameData.MaxBonusRatio);
                uint result = Math.Min(bonusAmount, maxBonus);
                return result;
            }

            public uint CalcReward(uint moveLeft)
            {
                return Agent._boardData.reward + CalcBonusCoin(moveLeft);
            }

            protected override void OnExit()
            {
                base.OnExit();
                if (!Agent.Restart)
                {
                    Agent.Exit?.Invoke(Agent._isLuxuryLevel);
                }

                Agent.DisposeBoard();
                _gameUi.Match3WinNextButtonClicked -= _gameUi_Match3WinNextButtonClicked;
                stateContext = null;
            }
        }

        #region Private Fields

        public event Action<BoardData, uint, uint, bool> WinMatch3Level;
        public event Action CheckForAds;
        public event Action<bool> Exit;
        private Board _board;
        BoardView boardView;

        [SerializeField] BoardData _boardData;
        [SerializeField] Camera _puzzleCamera;

        //[SerializeField] private Camera _match3Camera;
        private Fsm<PuzzleController> _fsm;
        // private GameUiManager _gameUi;
        // private AudioPlayer _audioPlayer;
        // private GameData _gameData;
        // private PlayerProfileV3 _profile;
        // private ProfileModule _profileModule;
        // private AdsManager _adsManager;

        public bool Restart = false;
        public bool ExitWithoutPlay = false;
        private bool _isRandomLevel;
        private int _extraMoves;

        public bool IsRandomLevel => _isRandomLevel;
        private bool _isLuxuryLevel;

        public static bool IsCouponEvent { get; private set; }


        public void DisposeBoard()
        {
            QuestGiver.Dispose();
            _board.Dispose();
        }

        public void SetCouponEvent(bool state)
        {
            IsCouponEvent = state;
        }

        public void SetBoardBoosters(List<BoosterType> boosterTypes)
        {
            _board.CacheInitialBoosters(boosterTypes);
        }

        public void SetBoardFingerBooster(BoosterType boosterType)
        {
            _board.CacheBoosterUnderFinger(boosterType);
        }

        public void UnSetBoardFingerBooster()
        {
            _board.ClearBoosterUnderFinger();
        }

        #endregion

        #region Unity

        protected override void Awake()
        {
            // MessagePackHelper.ConfigMessagePack();
            // TerminalMethods.AddObjectMethodsToTerminal(this);
            if (_puzzleCamera.GetComponent<CameraShake>() == null)
                _puzzleCamera.gameObject.AddComponent<CameraShake>();
        }

        private void CreateFsm()
        {
            InitState initState = new InitState {Name = "Puzzle Init"};
            EnterState enterState = new EnterState {Name = "Puzzle Enter"};
            PlayingState playState = new PlayingState {Name = "Puzzle Play"};
            ExitState exitState = new ExitState {Name = "Puzzle Exit"};

            Transition.CreateAndAssign(initState, enterState);
            Transition.CreateAndAssign(enterState, exitState, playState, new DelegatedCondition(() => ExitWithoutPlay));
            Transition.CreateAndAssign(playState, exitState);
            Transition.CreateAndAssign(exitState, initState, null, new DelegatedCondition(() => Restart));

            _fsm = new Fsm<PuzzleController>(this, initState);
            _fsm.Start();
        }

        public void Enter(BoardData data, GameData gameData, bool isChallengeAvailable = false, bool isRandomLevel = false, bool isLuxuryLevel = false)
        {
            _boardData = data;
            _boardData.Deserialize();
            _isRandomLevel = isRandomLevel;
            _isLuxuryLevel = isLuxuryLevel;
            // if (Context != null)
            // {
            //     _profileModule = Context.Get<ProfileModule>();
            //     _profile = _profileModule.GetLatestActiveProfile();
            //
            //     AudioModule audioModule = Context.Get<AudioModule>();
            //     _audioPlayer = audioModule.GetAudioPlayer();
            //     _gameData = gameData;
            //
            //     UiModule ui = Context.Get<UiModule>();
            //     _gameUi = ui.OpenWindow<GameUiManager>();
            //     _adsManager = Context.Get<AdsManager>();
            //     _gameUi.HomeEditorCanvas.Close();
            //     _gameUi.SceneUiController.Close();
            // }

            SetCouponEvent(isChallengeAvailable);
            CreateFsm();
        }

        private void WinTheGame(uint coinsWon, uint couponsWon)
        {
            //GameAnalytics.NewResourceEvent(GAResourceFlowType.Source, CurrencyName.Coin.ToString(), coinsWon, "Coin", "Match3CoinPrize");
            if (_isRandomLevel)
            {
                coinsWon = 0;
            }

            if (_isLuxuryLevel)
            {
                coinsWon = 0;
                couponsWon = 0;
            }

            // _profile.Wallet.Earn(CurrencyName.Coin, coinsWon, "Match3Level");
            // _profile.Wallet.Earn(CurrencyName.Coupon, couponsWon, "Match3Level", (_profile.State.LastMatch3LevelWon + 1));
            WinMatch3Level?.Invoke(_boardData, coinsWon, couponsWon, _isLuxuryLevel);
            _gameUi.HomeEditorCanvas.Open();
            _gameUi.OpenSceneUiController();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
            {
                DisposeBoard();
                _fsm.Start();
            }
#endif
            _fsm?.Update(Time.deltaTime);
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            QuestGiver.OnGUI();
        }
#endif

        #endregion
    }
}