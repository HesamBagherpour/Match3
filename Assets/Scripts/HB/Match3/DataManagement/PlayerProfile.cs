using System;
using System.Collections.Generic;
using HB.Match3.Economy;
using HB.Packages.Utilities;
using UnityEngine;

namespace HB.Match3.DataManagement
{

    [Serializable]
    public class PlayerProfile
    {
        public string Id;
        public DateTime LastVisitTime;
        public Wallet Wallet;
        public GameState State;
        public long uuid = -1;
        public string AuthKey = string.Empty;
        public string AbGroup = string.Empty;
        public int TutorialStepStarted = -1;
        public int TutorialStepFinished = -1;
        public List<ServiceData> servicesData = new List<ServiceData>();
        public uint launchCount;

        public virtual PlayerProfile CheckForNull()
        {
            return this;
        }
    }
    [Serializable]
    public class PlayerProfileV1 : PlayerProfile
    {
        public bool hasPurchasedOnce;
        public List<int> _stagesOpened;

        public PlayerProfileV1()
        {
            this._stagesOpened = new List<int>();
        }

        public override PlayerProfile CheckForNull()
        {

            if (_stagesOpened == null)
            {
                _stagesOpened = new List<int>();
            }

            return this;
        }

        public PlayerProfileV1(PlayerProfile playerProfile)
        {
            this._stagesOpened = new List<int>();
            this.uuid = playerProfile.uuid;
            this.Id = playerProfile.Id;
            this.launchCount = playerProfile.launchCount;
            this.servicesData = playerProfile.servicesData;
            this.State = playerProfile.State;
            this.Wallet = playerProfile.Wallet;
            this.AbGroup = playerProfile.AbGroup;
            this.AuthKey = playerProfile.AuthKey;
            this.LastVisitTime = playerProfile.LastVisitTime;
            this.TutorialStepFinished = playerProfile.TutorialStepFinished;
            this.TutorialStepStarted = playerProfile.TutorialStepStarted;

        }
        
        
    }
    
    
        [Serializable]
    public class PlayerProfileV2 : PlayerProfileV1
    {
        public int ProfileVersion;
        public Dictionary<string, int> IntVariables;
        public Dictionary<string, string> StringVariables;
        public Dictionary<String, DateTime> TimeVariable;


        public PlayerProfileV2()
        {
            ProfileVersion = 2;
            IntVariables = new Dictionary<string, int>();
            StringVariables = new Dictionary<string, string>();
            TimeVariable = new Dictionary<string, DateTime>();

        }

        public PlayerProfileV2(PlayerProfileV1 playerProfile)
        {
            this.ProfileVersion = 2;
            this.IntVariables = new Dictionary<string, int>();
            this.StringVariables = new Dictionary<string, string>();
            this.TimeVariable = new Dictionary<string, DateTime>();
            this._stagesOpened = playerProfile._stagesOpened;
            this.uuid = playerProfile.uuid;
            this.Id = playerProfile.Id;
            this.launchCount = playerProfile.launchCount;
            this.servicesData = playerProfile.servicesData;
            this.State = playerProfile.State;
            this.Wallet = playerProfile.Wallet;
            this.AbGroup = playerProfile.AbGroup;
            this.AuthKey = playerProfile.AuthKey;
            this.LastVisitTime = playerProfile.LastVisitTime;
            this.TutorialStepFinished = playerProfile.TutorialStepFinished;
            this.TutorialStepStarted = playerProfile.TutorialStepStarted;

        }


        public override PlayerProfile CheckForNull()
        {

            if (IntVariables == null)
            {
                //  Debug.LogError("Int is null at 2");
                ProfileVersion = 2;
                IntVariables = new Dictionary<string, int>();
                StringVariables = new Dictionary<string, string>();
                TimeVariable = new Dictionary<string, DateTime>();
            }
            if (_stagesOpened == null)
            {
                _stagesOpened = new List<int>();
            }

            return this;
        }
    }
    
        [Serializable]
    public class PlayerProfileV3 : PlayerProfileV2
    {
        public List<ProfilePurchaseData> ProfilePurchaseDatas;
        public List<ChallengeEventDataSaveable> ChallengeEventDataSaveables = new List<ChallengeEventDataSaveable>();

        //HB
        // public List<ProfileSceneData> ProfileSceneDatas = new List<ProfileSceneData>();
        // public List<ProfileEventData> ProfileEventDatas = new List<ProfileEventData>();
        // public List<ProfileLuxeryData> ProfileLuxeryDatas = new List<ProfileLuxeryData>();
        
        //HB 
        public UserConfig UserConfig;

        public override PlayerProfile CheckForNull()
        {
            if (ChallengeEventDataSaveables == null)
            {
                ChallengeEventDataSaveables = new List<ChallengeEventDataSaveable>();
            }
            if (ProfilePurchaseDatas == null)
            {
                ProfilePurchaseDatas = new List<ProfilePurchaseData>();
            }
            if (IntVariables == null)
            {
                // Debug.LogError("Int is null at 3");
                ProfileVersion = 2;
                IntVariables = new Dictionary<string, int>();
                StringVariables = new Dictionary<string, string>();
                TimeVariable = new Dictionary<string, DateTime>();
            }
            if (_stagesOpened == null)
            {
                _stagesOpened = new List<int>();
            }


            // for (int i = 0; i < Strings.AllIntVariables.Count(); i++)
            // {
            //     if (!IntVariables.ContainsKey(Strings.AllIntVariables[i]))
            //         IntVariables.Add(Strings.AllIntVariables[i], 0);
            // }

            // if (ProfileEventDatas == null)
            //     ProfileEventDatas = new List<ProfileEventData>();
            // if (ProfileSceneDatas == null)
            //     ProfileSceneDatas = new List<ProfileSceneData>();
            // if (ProfileLuxeryDatas == null)
            //     ProfileLuxeryDatas = new List<ProfileLuxeryData>();


            return this;
        }
        public PlayerProfileV3(PlayerProfileV2 playerProfile)
        {
            this.ProfileVersion = 2;
            this.IntVariables = new Dictionary<string, int>();
            this.StringVariables = new Dictionary<string, string>();
            this.TimeVariable = new Dictionary<string, DateTime>();
            this._stagesOpened = playerProfile._stagesOpened;
            this.uuid = playerProfile.uuid;
            this.Id = playerProfile.Id;
            this.launchCount = playerProfile.launchCount;
            this.servicesData = playerProfile.servicesData;
            this.State = playerProfile.State;
            this.Wallet = playerProfile.Wallet;
            this.AbGroup = playerProfile.AbGroup;
            this.AuthKey = playerProfile.AuthKey;
            this.LastVisitTime = playerProfile.LastVisitTime;
            this.TutorialStepFinished = playerProfile.TutorialStepFinished;
            this.TutorialStepStarted = playerProfile.TutorialStepStarted;
            this.ProfileVersion = playerProfile.ProfileVersion;
            this.IntVariables = playerProfile.IntVariables;
            this.StringVariables = playerProfile.StringVariables;
            this.TimeVariable = playerProfile.TimeVariable;

        }
        public PlayerProfileV3()
        {

        }
        public void SetIntVariable(string key, int value)
        {
            if (IntVariables == null)
                IntVariables = new Dictionary<string, int>();
            if (!IntVariables.ContainsKey(key))
                IntVariables.Add(key, 0);
            IntVariables[key] = value;
        }

    }
    
    
       [Serializable]
    public class ProfilePurchaseData
    {
       public string ProductID = "";
       public DateTime PurchaseDate;
       public int Match3Level;
       public uint PricePayed;
       public int PurchaseNumber;
       public string PurchaseID = "";
       public string ABGroup;
       public int PurchaseDay;
       public int PurchaseMonth;
       public int PurchaseYear;
        public int PurchaseHour;
        public bool IsEventActive = false;
        public int PurchaseMinute = 0;
        public string Market = "";
        public string InstallSource = "";
        public int CurrentPlayerSessionLength = 0;
        public int CurrentPlayerSessionCount = 0;
        public int CurrentMatch3Retries = 0;

        public string CurrentAppVersion = "";


        public ProfilePurchaseData()
        {

        }
        public ProfilePurchaseData(DateTime purchaseDate, string productId, uint pricePayed, PlayerProfileV3 playerProfileV3, string purchaseId, bool isEventActive, int count = -1)
        {

            PurchaseDate = purchaseDate;
            if (playerProfileV3.StringVariables.ContainsKey("Market"))
                Market = playerProfileV3.StringVariables["Market"];
            else
                Market = "CafeBazaar";
            PurchaseDay = purchaseDate.Day;
            PurchaseHour = purchaseDate.Hour;
            PurchaseMonth = purchaseDate.Month;
            PurchaseYear = purchaseDate.Year;
            PurchaseID = purchaseId;
            ProductID = productId;
            Match3Level = playerProfileV3.State.LastMatch3LevelWon;
            PricePayed = pricePayed;
            ABGroup = playerProfileV3.AbGroup;
            if (count == -1)
                PurchaseNumber = playerProfileV3.ProfilePurchaseDatas.Count + 1;
            else
                PurchaseNumber = count;

            if (!playerProfileV3.IntVariables.ContainsKey(Strings.TotalSessionCount))
                playerProfileV3.IntVariables.Add(Strings.TotalSessionCount, 0);
            CurrentPlayerSessionCount = playerProfileV3.IntVariables[Strings.TotalSessionCount];


            if (!playerProfileV3.IntVariables.ContainsKey(Strings.TotalSessionsLengthMinute))
                playerProfileV3.IntVariables.Add(Strings.TotalSessionsLengthMinute, 0);
            CurrentPlayerSessionLength = playerProfileV3.IntVariables[Strings.TotalSessionsLengthMinute];


            if (!playerProfileV3.StringVariables.ContainsKey(Strings.InstallSource))
                playerProfileV3.StringVariables.Add(Strings.InstallSource, "");
            InstallSource = playerProfileV3.StringVariables[Strings.InstallSource];


            IsEventActive = isEventActive;

            if (!playerProfileV3.IntVariables.ContainsKey(Strings.CurrentMatch3Retries))
                playerProfileV3.IntVariables.Add(Strings.CurrentMatch3Retries, 0);
            CurrentMatch3Retries = playerProfileV3.IntVariables[Strings.CurrentMatch3Retries];


            CurrentAppVersion = Application.version;

        }

        public void SetTime(DateTime purchaseDate)
        {
            PurchaseDate = purchaseDate;

            PurchaseDay = purchaseDate.Day;
            PurchaseHour = purchaseDate.Hour;
            PurchaseMonth = purchaseDate.Month;
            PurchaseYear = purchaseDate.Year;
            Debug.Log("purchase time set to " + purchaseDate);
        }



    }
    
    
    
    
        [Serializable] 
        public class ChallengeEventDataSaveable
        {
        
        public long Id;
        public string Name;
        public int InitiateMinuite;
        public int InitiateHour;
        public int InitiateDay;
        public int InitiateMonth;
        public int InitiateYear;
        public bool IsFixedEvent; // all players will experience this event after passing match3level
        
        public int ExpirationMinuite;
        public int ExpirationHour;
        public int ExpirationDay;
        public int ExpirationMonth;
        public int ExpirationYear;

        
        public int MaxMatch3Level;
        public int MinMatch3Level;


        public ChallengeEventDataSaveable(ChallengeEventData challengeEventData)
        {
            this.Id = challengeEventData.Id;
            this.Name = challengeEventData.Name;
            
            this.InitiateMinuite = challengeEventData.InitiateMinuite;
            this.InitiateHour = challengeEventData.InitiateHour;
            this.InitiateDay = challengeEventData.InitiateDay;
            this.InitiateMonth = challengeEventData.InitiateMonth;
            this.InitiateYear = challengeEventData.InitiateYear;
            
            this.ExpirationMinuite = challengeEventData.ExpirationMinuite;
            this.ExpirationHour = challengeEventData.ExpirationHour;
            this.ExpirationDay = challengeEventData.ExpirationDay;
            this.ExpirationMonth = challengeEventData.ExpirationMonth;
            this.ExpirationYear = challengeEventData.ExpirationYear;

            this.MaxMatch3Level = challengeEventData.MaxMatch3Level;
            this.MinMatch3Level = challengeEventData.MinMatch3Level;
            this.IsFixedEvent = challengeEventData.IsFixedEvent;
            }

            public ChallengeEventDataSaveable()
            {
                
            }
            
        }
        
        [Serializable]
        public class UserConfig
        {
            public string ShopName;
            public string GameDataName;

            public UserConfig()
            {
                ShopName = "Default";
                GameDataName = "Default";
            }
        }
    
    

}