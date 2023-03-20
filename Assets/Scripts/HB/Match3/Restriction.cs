using System;
using Garage.Match3;
using MessagePack;

namespace HB.Match3
{
    [Serializable]
    [MessagePackObject]
    public struct Lock
    {
        [Key(0)] public ActionType actionType;
        [Key(1)] public Direction direction;

        public bool Contains(ActionType act, Direction dir)
        {
            return (actionType & act) == act &&
                   (direction & dir) == dir;
        }
    }

    [Serializable]
    [MessagePackObject]
    public class Restriction
    {
        #region Public Fields

        [Key(0)] public Lock[] locks;
        [Key(1)] public HitType hitType;
        [Key(2)] public int health = 1;
        [Key(3)] public int order;


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