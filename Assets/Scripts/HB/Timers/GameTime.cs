using System;
using UnityEngine;

namespace HB.Timers
{
    public abstract class GameTime 
    {
        public static bool AllowLocalTime;
        public static long ElapsedTime()
        {
            if (Application.platform != RuntimePlatform.Android) return 0;
            AndroidJavaClass systemClock = new AndroidJavaClass("android.os.SystemClock");
            return systemClock.CallStatic<long>("elapsedRealtime");
        }
        
        
        public static DateTime Now()
        {
            return DateTime.Now.ToUniversalTime();
        }
    }
}
