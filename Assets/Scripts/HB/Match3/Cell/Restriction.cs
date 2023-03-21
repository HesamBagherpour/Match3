using System;

namespace HB.Match3.Cell
{
    [Serializable]

    public struct Lock
    {
         public ActionType actionType;
         public Direction direction;

        public bool Contains(ActionType act, Direction dir)
        {
            return (actionType & act) == act &&
                   (direction & dir) == dir;
        }
    }

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