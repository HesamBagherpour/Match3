using System;
using System.Collections.Generic;
using UnityEngine;

namespace HB.Match3.DataManagement
{
    [CreateAssetMenu(fileName = "ChallengeEventData", menuName = "ChallengeEventData", order = 1)]
    public class ChallengeEventData : ScriptableObject
   {
       public long Id;
       public string Name;
       public string AssetsKey;
       public SceneDialogueData AnnounceDialogue;
       public int InitiateMinuite;
       public int InitiateHour;
       public int InitiateDay;
       public int InitiateMonth;
       public int InitiateYear;
       public bool IsFixedEvent; // all players will experience this event after passing match3level



       public DateTime InitiateDate
       {
           get
           {
               if (_initateDate == null
                   || _initateDate.Day != InitiateDay
                   || _initateDate.Year != InitiateYear
                   || _initateDate.Month != InitiateMonth
                   || _initateDate.Hour != InitiateHour
                   || _initateDate.Minute != InitiateMinuite)
                   _initateDate = new DateTime(InitiateYear, InitiateMonth, InitiateDay, InitiateHour, InitiateMinuite,
                       0);
               return _initateDate;
           }
       }

       private DateTime _initateDate;
       public int ExpirationMinuite;
       public int ExpirationHour;
       public int ExpirationDay;
       public int ExpirationMonth;
       public int ExpirationYear;
       

       private DateTime _expirationDate;
       public DateTime ExpirationDate
       {
           get
           {
               if(_expirationDate == null 
                  || _expirationDate.Day != ExpirationDay 
                  || _expirationDate.Year != ExpirationYear 
                  || _expirationDate.Month != ExpirationMonth
                  || _expirationDate.Hour != ExpirationHour
                  || _expirationDate.Minute != ExpirationMinuite)
                   _expirationDate = new DateTime(ExpirationYear, ExpirationMonth, ExpirationDay,ExpirationHour,ExpirationMinuite,0);
               return _expirationDate;
           }
       }

       

       public long DownloadSize = -1;

       public int MaxMatch3Level;
       public int MinMatch3Level;
       
     
       public void Init(ChallengeEventDataSaveable challengeEventData)
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
       
       
   }
    
    [CreateAssetMenu]
    public class SceneDialogueData : ScriptableObject
    {
        public List<DlgBlockData> DlgBlockDataList;

        public void Init(List<DlgBlockData> excelData)
        {
            DlgBlockDataList = excelData;
        }

    }

    [Serializable]
    public class DlgBlockData
    {
        public string BlockName;
        public List<DlgSequenceData> SequenceDataList;

        public string AdjustEventOnStart;
        public string AnalaticsEventOnStart;

    }
    
    [System.Serializable]
    public class DlgSequenceData
    {
        public string SeqName;
        public SequencePosition PositionOnPage;
        public Direction EntranceDirection;
        public List<DlgLineData> LineDataList;
        

    }

    public enum SequencePosition
    {
        Left,
        Right
    }

    public enum Direction
    {
        Vertical,
        Horizontal,
    }
    
    [System.Serializable]
    public class DlgLineData
    {

        public string CharacterName;
        public string CharacterShowName;
        public string CharImageName;
        public Sprite CharacterImage;
        [TextArea(4, 20)] public string DlgLineText;
        public string AdjustEventOnShow;
        public string AnalaticsEventOnShow;
    }

}