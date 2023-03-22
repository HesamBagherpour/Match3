using System;
using System.Collections.Generic;

namespace HB.Match3.DataManagement
{
    [Serializable]
    public class StageState
    {
       public Dictionary<string, ActionSetState>  ActionStates;
       public Dictionary<string, DlgBlockState> DlgBlockStates;

       public bool IsLevelCompleted;
       public bool IsLevelOpen = false;
       public string Name;
       public bool IsBlueprintShown;

       public int Index;
       public bool IsLevelExpired;
       public string LastDialogueBlockPlayed;
       public bool IsChallenge = false;

        public StageState()
        {
            ActionStates = new Dictionary<string, ActionSetState>();
            DlgBlockStates = new Dictionary<string, DlgBlockState>();
        }
    } 
    
    public class ActionSetState
    {
        public bool IsPurchased;
        public int AppliedIndex = 0;
        public string Name;
        public bool IsLuxuryPurchased;
        public bool HasLuxuryVariant;
    }
    public class DlgBlockState
    {
        public bool IsBlockViewed = false;
    }
}