using System;
using HB.Match3.Cell;

namespace HB.Match3.Modules
{
    [Serializable]
    public class Restriction
    {
        #region Public Fields

        public Lock[] locks;
        public HitType hitType;
        public int health = 1;
        public int order;


        public bool Contains(ActionType act, Direction dir)
        {
            if (locks == null) return false;
            for (int i = 0; i < locks.Length; i++)
            {
                if (locks[i].Contains(act, dir)) return true;
            }

            return false;
        }

        #endregion
    }
}