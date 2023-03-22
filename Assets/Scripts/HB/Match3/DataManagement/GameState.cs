using System;
using System.Collections.Generic;

namespace HB.Match3.DataManagement
{
    [Serializable]
    public class GameState
    {
        public bool IsInitialized;
        public string LatestLoadedLevel = "";
        public Dictionary<string, StageState> StageStates;
        public int LastMatch3LevelWon = 0;
        public bool IsMusicOn=true;
        public bool IsSfxOn=true;
        public DateTime EnergyCollectStartTime=DateTime.MinValue;
        public DateTime InfiniteEnergyEndTime = DateTime.MinValue;


        public GameState()
        {
            StageStates = new Dictionary<string, StageState>();
        }
 
    }
}