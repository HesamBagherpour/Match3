using System;

namespace HB.Packages.Utilities
{
       public static class Strings
    {
        public const string GameData = "GameData";
        public const string MainMenu = "MainMenu";
        public const string PreloadScene = "PreloadScene";
        public const string LoadingScene = "LoadingScene";
        public const string AppSetting = "AppSetting";
        public const string Match3Debug = "Match3Debug";
        public const string BoardViewData = "board-view-data";
        public const string Match3LevelData = "Match3LevelData";
        public const string InstallSource = "InstallSource";
        
        public const string LastNormalLevel = "LastNormalLevel";
        public const string LastChallengeLevel = "LastChallengeLevel";
        public const string CurrentShopSource = "CurrentShopSource";
        public const string IfShopOpenedEvent = "IfShopOpenedEvent";
        
        
        public const string Monetization = "Monetization";
        public const string Design = "Design";
        public const string Technical = "Technical";
        public const string Resource = "Resource";
        public const string Tutorial = "Tutorial";
        
        public const string Move = "Move";
        public const string Project = "Project";
        public const string Challenge = "Challenge";
        public const string DailyAward = "DailyAward";
        public const string Ad = "Ad";
        public const string Event = "Event";
        
        
            
        public const string Match3Level = "Match3Level";


     	public const string NumberOfTries = "NumberOfTries";
        public const string NumberOfTimesMovePurchased = "NumberOfTimesMovePurchased";
        public const string NumberOfInGameBoosterUsed = "NumberOfInGameBoosterUsed";
        public const string NumberOFPreGameBoosterUsed = "NumberOFPreGameBoosterUsed";
        
        public const string InBoosterPurchaseWindowShown = "InBoosterPurchaseWindowShown";
        public const string PurchaseInBoosterButtonPressed = "PurchaseInBoosterButtonPressed";
        public const string InBoosterShopWindowShown = "InBoosterShopWindowShown";
        public const string InBoosterPurchaseSuccess = "InBoosterPurchaseSuccess";
        
        
        
        public const string PreBoosterPurchaseWindowShown = "PreBoosterPurchaseWindowShown";
        public const string PurchasePreBoosterButtonPressed = "PurchasePreBoosterButtonPressed";
        public const string PreBoosterShopWindowShown = "PreBoosterShopWindowShown";
        public const string PreBoosterPurchaseSuccess = "PreBoosterPurchaseSuccess";
        public const string ShopOfferPurchaseSuccess = "ShopOfferPurchaseSuccess";
        
        
        public const string OutOfMoveWindowShown = "OutOfMoveWindowShown";
        public const string PurchaseMoveButtonPressed = "PurchaseMoveButtonPressed";
        public const string MoveShopWindowShown = "MoveShopWindowShown";
        public const string MovePurchaseSuccess = "MovePurchaseSuccess";
        
        
        public const string OutOfEnergyWindowShown = "OutOfEnergyWindowShown";
        public const string RefillEnergyButtonPressed = "RefillEnergyButtonPressed";
        public const string EnergyShopWindowShown = "EnergyShopWindowShown";
        public const string EnergyPurchaseSuccess = "EnergyPurchaseSuccess";
        
        
            
        
        
        private static readonly string[] SizeSuffixes =
            {"bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"};

        #region IntVariables

        public const string TotalSessionCount = "TotalSessionCount";
        public const string ThisSessionTimePassedSync = "ThisSessionTimePassedSync";
        public const string TotalSessionsLengthMinute = "TotalSessionsLengthMinute";
        public const string NumberOfMatch3Retries = "NumberOfMatch3Retries";
        public const string AdsWatched = "AdsWatched";
        public const string JoinVersion = "JoinVersion";
        public const string HighestHomeDesignLevel = "HighestHomeDesignLevel";
        
        
        public const string AdsWatchedMainPageCount = "AdsWatchedMainPageCount";
        public const string AdsWatchedMatch3StartCount = "AdsWatchedMatch3StartCount";
        public const string AdsWatchedMatch3Lose = "AdsWatchedMatch3Lose";
        public const string AdsWatchedMatch3Finish = "AdsWatchedMatch3Finish";
            
        
        public const string TotalGemProductPurchases = "TotalGemProductPurchases";
        public const string TotalBundleProductPurchases = "TotalBundleProductPurchases";
        public const string TotalEarnedGemByProductPurchases = "TotalEarnedGemByProductPurchases";
        public const string TotalEarnedGemByPlaying = "TotalEarnedGemByPlaying";
        public const string TotalCoinEarnedByPlaying = "TotalCoinEarnedByPlaying";
        public const string TotalPowerupsEarnedByPlaying = "TotalPowerupsEarnedByPlaying";
        
        
        public const string GemPurchasesSumTooman = "GemPurchasesSumTooman";
        public const string BundlePurchasesSumTooman = "BundlePurchasesSumTooman";
        
        public const string ShopVisitsCount = "ShopVisitsCount";
        public const string ShopOfferVisitsCount = "ShopOfferVisitsCount";
        public const string FurniturePlacedCount = "FurniturePlacedCount";
        public const string DailyRewardsReceived = "DailyRewardsReceived";


        public const string TotalEventsExpired = "TotalEventsExpired"; 
        
        public const string TotalGemsUsedOnMoreMove = "TotalGemsUsedOnMoreMove";
        public const string TotalGemsUsedOnHammer = "TotalGemsUsedOnHammer";
        public const string TotalGemsUsedOnInCross = "TotalGemsUsedOnInCross";
        public const string TotalGemsUsedOnInRainbow = "TotalGemsUsedOnInRainbow";
        public const string TotalGemsUsedOnStar = "TotalGemsUsedOnStar";
        public const string TotalGemsUsedOnJumbo = "TotalGemsUsedOnJumbo";
        public const string TotalGemsUsedOnPreCross = "TotalGemsUsedOnPreCross";
        public const string TotalGemsUsedOnPreRainbow = "TotalGemsUsedOnPreRainbow";
        public const string TotalGemsUsedOnRefillEnergy = "TotalGemsUsedOnRefillEnergy";
        public const string TotalGemsUsedOnEventsReopen = "TotalGemsUsedOnEventsReopen";

        public const string TotalUsedPreRainbow = "TotalUsedPreRainbow";
        public const string TotalUsedHammer = "TotalUsedHammer";
        public const string TotalUsedInCross = "TotalUsedInCross";
        
        
        public const string TotalUsedInRainbow = "TotalUsedInRainbow";
        public const string TotalUsedStar = "TotalUsedStar";
        public const string TotalUsedJumbo = "TotalUsedJumbo";
        
        public const string TotalUsedPreCross = "TotalUsedPreCross";
        public const string NumberOfTimesEnergyDepleted = "NumberOfTimesEnergyDepleted";
        
        public const string CurrentHintIndex = "CurrentHintIndex";

        public const string CurrentMatch3Retries = "CurrentMatch3Retries";
        public const string LastMatch3LevelPlayed = "LastMatch3LevelPlayed";

        
        public const string NumberLuxuryStarted = "NumberLuxuryStarted";
        public const string NumberLuxuryFinished = "NumberLuxuryFinished";
        public const string TotalStarsEarned = "NumberStarsAquired";
        public const string TotalLuxEarned = "TotalLuxEarned";
        
        public const string LuxuryStartNotifID = "LuxuryStartNotifID";
        public const string LuxuryExpireNotifID = "LuxuryExpireNotifID";
        



        public static readonly string[] AllIntVariables = new string[39]
        {
            TotalSessionCount,
            ThisSessionTimePassedSync,
            TotalSessionsLengthMinute,
            NumberOfMatch3Retries,
            AdsWatched,
            HighestHomeDesignLevel,
            AdsWatchedMainPageCount,
            AdsWatchedMatch3StartCount,
            AdsWatchedMatch3Lose,
            AdsWatchedMatch3Finish,
            TotalGemProductPurchases,
            TotalBundleProductPurchases,
            TotalEarnedGemByProductPurchases,
            TotalEarnedGemByPlaying,
            TotalCoinEarnedByPlaying,
            TotalPowerupsEarnedByPlaying,
            GemPurchasesSumTooman,
            BundlePurchasesSumTooman,
            ShopVisitsCount,
            ShopOfferVisitsCount,
            FurniturePlacedCount,
            DailyRewardsReceived,
            TotalGemsUsedOnMoreMove,
            TotalGemsUsedOnHammer,
            TotalGemsUsedOnInCross,
            TotalGemsUsedOnInRainbow,
            TotalGemsUsedOnStar,
            TotalGemsUsedOnJumbo,
            TotalGemsUsedOnPreCross,
            TotalGemsUsedOnPreRainbow,
            TotalGemsUsedOnRefillEnergy,
            TotalGemsUsedOnEventsReopen,
            TotalUsedPreRainbow,
            TotalUsedHammer,
            TotalUsedInCross,
            TotalUsedInRainbow,
            TotalUsedStar,
            TotalUsedJumbo,
            TotalUsedPreCross
        };
        
       
        #endregion
        
        #region TimeVariables
        public const string JoinTime = "JoinTime";
        public const string ServerJoinTime = "ServerJoinTime";
        public const string ServerLastLoginTime = "ServerLastLoginTime";
        public const string ClientLastLoginTime = "ClientLastLoginTime";
        #endregion

        public static string SizeSuffix(long value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(decimalPlaces));
            }

            if (value < 0)
            {
                return "-" + SizeSuffix(-value);
            }

            if (value == 0)
            {
                return string.Format("{{0:n" + decimalPlaces + "}} bytes", 0);
            }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int) Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal) value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }


            return string.Format("{{0:n" + decimalPlaces + "}} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
    }


    public enum CurrencyName
    {
        Coin,
        Gem,
        Coupon,
        Energy,
        PreGameBlasterPowerUp,
        InGameBlasterPowerUp,
        HammerPowerUp,
        PreGameRainbowPowerUp,
        InGameRainbowPowerUp,
        SquarePowerUp,
        FatPowerUp,
        ExtraMovePack,
        LuxuryDollar
   

    }
}