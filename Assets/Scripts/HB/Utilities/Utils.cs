using System;
using Garage.Match3;
using HB.Match3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HB.Utilities
{
    public class Utils
    {
        #region Public Methods

        private const float SwipeThreshold = 0.5f;

        public static T GetRandomEnum<T>(int startIndex = -1)
        {
            Array values = Enum.GetValues(typeof(T));
            if (values.Length == 0) return default;
            if (values.Length == 1)
                return (T) values.GetValue(0);

            return (T) values.GetValue(
                Random.Range(startIndex == -1 ? 0 : startIndex,
                    values.Length));
        }

        public static Direction GetDirection(Vector2 dir)
        {
            dir = dir.normalized;
            float absDistanceX = Mathf.Abs(dir.x);
            float absDistanceY = Mathf.Abs(dir.y);
            Direction swipeDir = Direction.Center;
            if (absDistanceX > SwipeThreshold || absDistanceY > SwipeThreshold)
            {
                if (absDistanceX > absDistanceY)
                {
                    if (dir.x > 0) swipeDir = Direction.Right;
                    else swipeDir = Direction.Left;
                }
                else
                {
                    if (dir.y > 0) swipeDir = Direction.Top;
                    else swipeDir = Direction.Bottom;
                }
            }

            return swipeDir;
        }
        #endregion
    }
}