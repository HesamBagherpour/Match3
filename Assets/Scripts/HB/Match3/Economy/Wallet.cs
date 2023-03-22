using System;
using System.Collections.Generic;
using HB.Match3.DataManagement;
using HB.Packages.Timers;
using HB.Packages.Utilities;
using UnityEngine;
using CurrencyName = HB.Match3.DataManagement.CurrencyName;

namespace HB.Match3.Economy
{

    [Serializable]
    public class Wallet : ISerializationCallbackReceiver
    {

        [Serializable]
        public class CurrencyObject
        {
            public string _currencyName;
            public uint _amount;

            public CurrencyObject(string name, uint amount)
            {
                _currencyName = name;
                _amount = amount;
            }

        }

        public Dictionary<CurrencyName, Currency> _currencies;
        public event Action<CurrencyName, uint> CurrencyChanged;
        private Dictionary<CurrencyName, DateTime> _lockedCurrencies;
        public List<CurrencyObject> _currencyObjects;
        private PlayerProfileV3 _playerProfileV3;


        public Wallet()
        {
            _currencies = new Dictionary<CurrencyName, Currency>();
            _lockedCurrencies = new Dictionary<CurrencyName, DateTime>();
            _currencyObjects = new List<CurrencyObject>();
            var length = Enum.GetNames(typeof(CurrencyName)).Length;
            for (int i = 0; i < length; i++)
            {
                _currencyObjects.Add(new CurrencyObject(((CurrencyName)i).ToString(), 0));
            }
        }

        public void Init(PlayerProfileV3 playerProfileV3)
        {
            this._playerProfileV3 = playerProfileV3;
        }
        public void Print()
        {
            foreach (var currency in _currencies)
            {
                //Debug.Log(currency.Key + ":" + currency.Value.Amount + "\n");
            }
        }

        public bool Earn(CurrencyName currency, uint amount, string source, int match3Level = -1)
        {
            if (_currencies.TryGetValue(currency, out Currency c) == false)
            {
                c = new Currency() { Name = currency.ToString() };
                _currencies.Add(currency, c);

            }


            if (source.Equals("PreGameReleased"))
            {
                switch (currency)
                {
                    case CurrencyName.PreGameBlasterPowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedPreCross))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedPreCross, 0);

                        _playerProfileV3.IntVariables[Strings.TotalUsedPreCross] -= (int)amount;
                        break;
                    case CurrencyName.InGameBlasterPowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedInCross))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedInCross, 0);
                        _playerProfileV3.IntVariables[Strings.TotalUsedInCross] -= (int)amount;
                        break;
                    case CurrencyName.HammerPowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedHammer))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedHammer, 0);

                        _playerProfileV3.IntVariables[Strings.TotalUsedHammer] -= (int)amount;

                        break;
                    case CurrencyName.PreGameRainbowPowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedPreRainbow))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedPreRainbow, 0);

                        _playerProfileV3.IntVariables[Strings.TotalUsedPreRainbow] -= (int)amount;
                        break;
                    case CurrencyName.InGameRainbowPowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedInRainbow))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedInRainbow, 0);
                        _playerProfileV3.IntVariables[Strings.TotalUsedInRainbow] -= (int)amount;

                        break;
                    case CurrencyName.SquarePowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedStar))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedStar, 0);

                        _playerProfileV3.IntVariables[Strings.TotalUsedStar] -= (int)amount;
                        break;
                    case CurrencyName.FatPowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedJumbo))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedJumbo, 0);
                        _playerProfileV3.IntVariables[Strings.TotalUsedJumbo] -= (int)amount;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(currency), currency, null);
                }
            }

            if (currency == CurrencyName.Gem)
            {
                if (source.Equals("Shop"))
                {
                    if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalEarnedGemByProductPurchases))
                        _playerProfileV3.IntVariables.Add(Strings.TotalEarnedGemByProductPurchases, (int)amount);
                    else
                        _playerProfileV3.IntVariables[Strings.TotalEarnedGemByProductPurchases] += (int)amount;
                }
                else if(!source.Equals("Tutorial"))
                {
                    Debug.Log("gem added from non tutorial source");
                    if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalEarnedGemByPlaying))
                        _playerProfileV3.IntVariables.Add(Strings.TotalEarnedGemByPlaying, (int)amount);
                    else
                        _playerProfileV3.IntVariables[Strings.TotalEarnedGemByPlaying] += (int)amount;
                }

            }


            if (currency == CurrencyName.Coin)
            {
                if (!source.Equals("Shop"))
                {
                    if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalCoinEarnedByPlaying))
                        _playerProfileV3.IntVariables.Add(Strings.TotalCoinEarnedByPlaying, (int)amount);
                    else
                        _playerProfileV3.IntVariables[Strings.TotalCoinEarnedByPlaying] += (int)amount;
                }
            }

            if (currency == CurrencyName.FatPowerUp ||
                currency == CurrencyName.HammerPowerUp ||
                currency == CurrencyName.SquarePowerUp ||
                currency == CurrencyName.InGameBlasterPowerUp ||
                currency == CurrencyName.InGameRainbowPowerUp ||
                currency == CurrencyName.PreGameBlasterPowerUp ||
                currency == CurrencyName.PreGameRainbowPowerUp)
            {
                if (!source.Equals("Shop") && !source.Equals("Tutorial"))
                {
                    if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalPowerupsEarnedByPlaying))
                        _playerProfileV3.IntVariables.Add(Strings.TotalPowerupsEarnedByPlaying, (int)amount);
                    else
                        _playerProfileV3.IntVariables[Strings.TotalPowerupsEarnedByPlaying] += (int)amount;
                }
                
            }




            if (currency == CurrencyName.Gem)
            {
                if (!source.Equals("Tutorial"))
                {
                    if (match3Level != -1)
                    {
                        //GameAnalytics.NewDesignEvent("Source:" + currency.ToString() + ":" + source+":"+(match3Level),amount);
                    }
                    else
                    {
                        //GameAnalytics.NewDesignEvent("Source:" + currency.ToString() + ":" + source,amount);
                        }
                }
            }
            else
            {
                if (match3Level != -1)
                {
                    //GameAnalytics.NewDesignEvent("Source:" + currency.ToString() + ":" + source+":"+(match3Level),amount);
                }
                else
                {
                   // GameAnalytics.NewDesignEvent("Source:" + currency.ToString() + ":" + source,amount);
                    }
            }
            




            if (!IsCurrencyLocked(currency) && c.Earn(amount))
                OnCurrencyChanged(currency, c);
            else
                return false;
            return true;
        }

        public DateTime? GetLastEarnTime(CurrencyName currency)
        {
            return _currencies.TryGetValue(currency, out Currency c) ? c.LastEarnTime : (DateTime?)null;
        }

        public DateTime? GetLastPayTime(CurrencyName currency)
        {
            return _currencies.TryGetValue(currency, out Currency c) ? c.LastPayTime : (DateTime?)null;
        }

        public uint GetCapacity(CurrencyName currency)
        {
            return _currencies.TryGetValue(currency, out Currency c) ? c.Capacity : 0;
        }

        public void SetCapacity(CurrencyName currency, uint capacity)
        {
            if (_currencies.TryGetValue(currency, out Currency c))
            {
                c.Capacity = capacity;
                c.Amount = (uint)Mathf.Clamp(c.Amount, 0, capacity);
            }
        }

        public bool Pay(CurrencyName currency, uint amount, string target, int match3Level = -1)
        {

            bool result = false;
            if (!IsCurrencyLocked(currency))
            {
                result = _currencies.TryGetValue(currency, out Currency c) && c.Pay(amount);
                if (result)
                {
                    OnCurrencyChanged(currency, c);
                }
            }

            if (!result)
                return false;
            
            if (currency == CurrencyName.Gem)
            {
                bool sendBoosterEvent = false;
                switch (target)
                {
                    case "MoreMove":
                    {
                        //Debug.Log(Strings.TotalGemsUsedOnMoreMove + " adding " + amount);
                        
                            if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalGemsUsedOnMoreMove))
                                _playerProfileV3.IntVariables.Add(Strings.TotalGemsUsedOnMoreMove, 0);

                            _playerProfileV3.IntVariables[Strings.TotalGemsUsedOnMoreMove] += (int)amount;
                            Debug.Log(Strings.TotalGemsUsedOnMoreMove + " in profile " + _playerProfileV3.IntVariables[Strings.TotalGemsUsedOnMoreMove]);
                            break;
                        }

                    case "EnergyRefill":
                        {
                            if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalGemsUsedOnRefillEnergy))
                                _playerProfileV3.IntVariables.Add(Strings.TotalGemsUsedOnRefillEnergy, 0);
                            _playerProfileV3.IntVariables[Strings.TotalGemsUsedOnRefillEnergy] += (int)amount;
                            break;
                        }
                    case "HammerPowerUp":
                    {
                        sendBoosterEvent = true;
                            if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalGemsUsedOnHammer))
                                _playerProfileV3.IntVariables.Add(Strings.TotalGemsUsedOnHammer, 0);
                            _playerProfileV3.IntVariables[Strings.TotalGemsUsedOnHammer] += (int)amount;
                            break;
                        }


                    case "PreGameBlasterPowerUp":
                        {
                            sendBoosterEvent = true;
                            if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalGemsUsedOnPreCross))
                                _playerProfileV3.IntVariables.Add(Strings.TotalGemsUsedOnPreCross, 0);
                            _playerProfileV3.IntVariables[Strings.TotalGemsUsedOnPreCross] += (int)amount;
                            break;
                        }

                    case "InGameBlasterPowerUp":
                        {
                            sendBoosterEvent = true;
                            if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalGemsUsedOnInCross))
                                _playerProfileV3.IntVariables.Add(Strings.TotalGemsUsedOnInCross, 0);
                            _playerProfileV3.IntVariables[Strings.TotalGemsUsedOnInCross] += (int)amount;
                            break;
                        }

                    case "PreGameRainbowPowerUp":
                        {
                            sendBoosterEvent = true;
                            if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalGemsUsedOnPreRainbow))
                                _playerProfileV3.IntVariables.Add(Strings.TotalGemsUsedOnPreRainbow, 0);
                            _playerProfileV3.IntVariables[Strings.TotalGemsUsedOnPreRainbow] += (int)amount;
                            break;
                        }


                    case "InGameRainbowPowerUp":
                        {
                            sendBoosterEvent = true;
                            if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalGemsUsedOnInRainbow))
                                _playerProfileV3.IntVariables.Add(Strings.TotalGemsUsedOnInRainbow, 0);
                            _playerProfileV3.IntVariables[Strings.TotalGemsUsedOnInRainbow] += (int)amount;
                            break;
                        }

                    case "SquarePowerUp":
                        {
                            sendBoosterEvent = true;
                            if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalGemsUsedOnStar))
                                _playerProfileV3.IntVariables.Add(Strings.TotalGemsUsedOnStar, 0);
                            _playerProfileV3.IntVariables[Strings.TotalGemsUsedOnStar] += (int)amount;
                            break;
                        }


                    case "FatPowerUp":
                        {
                            sendBoosterEvent = true;
                            if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalGemsUsedOnJumbo))
                                _playerProfileV3.IntVariables.Add(Strings.TotalGemsUsedOnJumbo, 0);
                            _playerProfileV3.IntVariables[Strings.TotalGemsUsedOnJumbo] += (int)amount;
                            break;
                        }
                }

                if (sendBoosterEvent)
                {
                   // if(!target.Equals("Tutorial"))
                        //GameAnalytics.NewDesignEvent("Sink:Gem:Booster:" + (_playerProfileV3.State.LastMatch3LevelWon + 1).ToString(),amount);
                }
                
            }


            if (target.Equals("PreGameSelect") || target.Equals("InGameUse"))
            {
                switch (currency)
                {
                    case CurrencyName.PreGameBlasterPowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedPreCross))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedPreCross, 0);

                        _playerProfileV3.IntVariables[Strings.TotalUsedPreCross] += (int)amount;
                        break;
                    case CurrencyName.InGameBlasterPowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedInCross))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedInCross, 0);

                        _playerProfileV3.IntVariables[Strings.TotalUsedInCross] += (int)amount;
                        break;
                    case CurrencyName.HammerPowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedHammer))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedHammer, 0);

                        _playerProfileV3.IntVariables[Strings.TotalUsedHammer] += (int)amount;

                        break;
                    case CurrencyName.PreGameRainbowPowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedPreRainbow))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedPreRainbow, 0);

                        _playerProfileV3.IntVariables[Strings.TotalUsedPreRainbow] += (int)amount;
                        break;
                    case CurrencyName.InGameRainbowPowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedInRainbow))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedInRainbow, 0);

                        _playerProfileV3.IntVariables[Strings.TotalUsedInRainbow] += (int)amount;
                        break;
                    case CurrencyName.SquarePowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedStar))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedStar, 0);

                        _playerProfileV3.IntVariables[Strings.TotalUsedStar] += (int)amount;
                        break;
                    case CurrencyName.FatPowerUp:
                        if (!_playerProfileV3.IntVariables.ContainsKey(Strings.TotalUsedJumbo))
                            _playerProfileV3.IntVariables.Add(Strings.TotalUsedJumbo, 0);

                        _playerProfileV3.IntVariables[Strings.TotalUsedJumbo] += (int)amount;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(currency), currency, null);
                }
                
               /// GameAnalytics.NewDesignEvent("Sink:Booster:Match3Level:" + (ServerDataSender.Instance.ProfileModule.GetLatestActiveProfile().State.LastMatch3LevelWon+1).ToString(),amount);
                
            }

            if ((target.Equals("EnergyRefill") ||target.Equals("OutOfMoveScreen")) && match3Level !=-1)
            {
                string match3LevelText = match3Level.ToString();
                if (match3Level < 1000 && match3Level > 99)
                    match3LevelText = "0" + match3LevelText;
                else if(match3Level < 100 && match3Level > 9)
                {
                    match3LevelText = "00" + match3LevelText;
                }
                else if(match3Level < 10)
                {
                    match3LevelText = "000" + match3LevelText;
                }
               // GameAnalytics.NewDesignEvent("Sink:" + currency.ToString() + ":" + target+":"+match3LevelText,amount);
            }
           // else
               // GameAnalytics.NewDesignEvent("Sink:" + currency.ToString() + ":" + target,amount);
            
            return result;
        }

        public uint GetAmount(CurrencyName currency)
        {
            return _currencies.TryGetValue(currency, out Currency c) ? c.Amount : 0;
        }

        private void OnCurrencyChanged(CurrencyName name, Currency c)
        {
            CurrencyChanged?.Invoke(name, c.Amount);
        }

        public void SetValue(CurrencyName currency, uint amount)
        {
            if (!IsCurrencyLocked(currency))
            {
                if (_currencies.TryGetValue(currency, out Currency c))
                {
                    c.Amount = amount;
                }
                else
                {
                    _currencies.Add(currency, new Currency() { Name = currency.ToString() });
                    SetValue(currency, amount);
                }
                OnCurrencyChanged(currency, c);
            }
        }
        public bool IsCurrencyLocked(CurrencyName currency)
        {
            if (_lockedCurrencies.TryGetValue(currency, out DateTime date))
            {
                if (date < GameTime.Now())
                    UnlockCurrency(currency);
                return date > GameTime.Now();
            }
            return false;
        }
        public void LockCurrency(CurrencyName currency, TimeSpan lockDuration)
        {
            if (!_lockedCurrencies.ContainsKey(currency))
                _lockedCurrencies.Add(currency, GameTime.Now().Add(lockDuration));

            CurrencyChanged?.Invoke(currency, GetAmount(currency));
        }
        public void UnlockCurrency(CurrencyName currency)
        {
            _lockedCurrencies.Remove(currency);
            CurrencyChanged?.Invoke(currency, GetAmount(currency));
        }

        public void OnBeforeSerialize()
        {
            foreach (var currency in _currencies)
            {
                _currencyObjects[(int)currency.Key]._currencyName = currency.Key.ToString();
                _currencyObjects[(int)currency.Key]._amount = currency.Value.Amount;

            }
        }

        public void OnAfterDeserialize()
        {

        }
    }


    public sealed class Currency
    {
        public string Name;
        public uint Amount;
        public uint Capacity;
        public DateTime LastEarnTime;
        public DateTime LastPayTime;

        public Currency()
        {
            Amount = 0;
        }

        public bool Earn(uint amount)
        {
            uint a1 = Amount;
            Amount += amount;
            if (AmountIsCapped() &&
                Amount > Capacity)
            {
                Amount = Capacity;
            }

            if (a1 == Amount) return false;
            LastEarnTime = GameTime.Now();
            return true;
        }

        private bool AmountIsCapped()
        {
            return Capacity > 0;
        }

        public bool Pay(uint amount)
        {
            if (amount > Amount) return false;
            Amount -= amount;
            LastPayTime = GameTime.Now();

            return true;
        }
    }
}