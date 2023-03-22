using System;
using HB.Match3.Models;
using UnityEngine;

namespace HB.Match3.DataManagement
{
    [Serializable]
    [CreateAssetMenu(menuName = "HB/PowerUpData")]
    public class PowerUpData:ScriptableObject
    {
        public CurrencyName CurrencyName;
        public BoosterType BoosterType;
        public Situation Situation;
        public Sprite PowerUpImage;
        public string ShowName;
        public string Description;
        public uint PriceByGem;
        public int NumberOfPacks;
        public int ItemsInPack;
        public string PowerUpBoughtAdjustEvent;
        public string PowerUpUsedAdjustEvent;
    }
}