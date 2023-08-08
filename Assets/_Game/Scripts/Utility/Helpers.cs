using System;
using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Utilities
{
    public static class Helpers
    {
        public static Vector2 xz(this Vector3 vv)
        {
            return new Vector2(vv.x, vv.z);
        }
        
        public static Vector2 xy(this Vector3 vv)
        {
            return new Vector2(vv.x, vv.z);
        }
     
        public static float FlatDistanceX(this Vector3 from, Vector3 unto)
        {
            Vector2 a = from.xz();
            Vector2 b = unto.xz();
            return Vector2.Distance( a, b );
        }
        
        public static bool Equals(double x, double y, double tolerance)
        {
            var diff = Math.Abs(x - y);
            return diff <= tolerance ||
                   diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
        }

        public static IEnumerator RepeatWithDelay(int repeatCount, float interval, Action action, Action finishCallBack = null)
        {
            var time = new WaitForSeconds(interval);

            for (int i = 0; i < repeatCount; i++)
            {
                action.Invoke();
                yield return time;
            }

            finishCallBack?.Invoke();
        }

        public static IEnumerator RepeatWithDelayStringArgument(
            int repeatCount,
            float interval,
            Action<string> action,
            string[] message,
            Action finishCallBack = null)
        {
            var time = new WaitForSeconds(interval);

            for (int i = 0; i < repeatCount; i++)
            {
                action?.Invoke(message[i]);
                yield return time;
            }

            finishCallBack.Invoke();
        }
    }
}
