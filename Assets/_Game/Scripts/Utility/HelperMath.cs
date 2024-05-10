using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Utility
{
    public static class HelperMath
    {
        public static int LenghtToCount(int value)
        {
            return (value - 1);
        }

        public static Vector3 GetRandomPosition(float min, float max, bool ignoreY = false)
        {
            return new Vector3(
                Random.Range(min, max),
                ignoreY ? 0 : Random.Range(min, max),
                Random.Range(min, max));
        }

        public static Vector3 GetRandomPositionWithClamp
        (
            float min,
            float max,
            bool ignoreY = false,
            float clampDistance = 0
        )
        {
            float randomX = Random.Range(min, max);
            float randomY = Random.Range(min, max);
            float randomZ = Random.Range(min, max);

            SetRandomClamp(ref randomX, clampDistance);
            SetRandomClamp(ref randomY, clampDistance);
            SetRandomClamp(ref randomZ, clampDistance);

            return new Vector3(
                randomX,
                ignoreY ? 0 : randomY,
                randomZ);

            //local methods
            void SetRandomClamp(ref float randomNum, float clamp)
            {
                clamp *= GetNumberSign(randomNum);

                if (randomNum < clamp)
                {
                    randomNum += clamp;
                }
            }
        }

        public static float Vector3DistanceMagnitude(Vector3 one, Vector3 two)
        {
            return (one - two).magnitude;
        }

        public static float Vector3DistanceSqrMagnitude(Vector3 one, Vector3 two)
        {
            return (float)Math.Pow((one - two).sqrMagnitude, 2);
        }

        /// <summary>
        /// returned number sign is plus number 1 if minus number -1
        /// return 1 or -1 if input number is low than 0 return -1 if higher than 0 +1
        /// </summary>
        /// <returns></returns>
        public static int GetNumberSign(float number) => number < 0 ? -1 : 1;

        public static int ClampMin(int value, int minValue) =>
            value < minValue ? minValue : value;

        public static float ClampMin(float value, float minValue) =>
            value < minValue ? minValue : value;

        public static int ClampMax(int value, int maxValue) =>
            value > maxValue ? maxValue : value;

        public static float ClampMax(float value, float maxValue) =>
            value > maxValue ? maxValue : value;

        public static Color AlphaToMax(Color color)
        {
            return new Color(color.r, color.g, color.b, 1);
        }

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
            return Vector2.Distance(a, b);
        }

        public static bool Equals(double x, double y, double tolerance)
        {
            var diff = Math.Abs(x - y);
            return diff <= tolerance ||
                   diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
        }
    }

    public static class ObservableHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="monoObject"></param> object from will done this call/
        /// <param name="repeatCount"></param> how much its will repeat/
        /// <param name="waitTime"></param> interval of the repeat/
        /// <param name="doAfterInit"></param> action whats will done after any time/
        /// <returns></returns>
        public static IDisposable WaitAndDoCycle
        (
            this MonoBehaviour monoObject,
            int repeatCount,
            float waitTime,
            Action<int> action,
            Action finishCycleAction = null
        )
        {
            IDisposable intervalDisposable = null;
            
            
            var i = 0;   
            intervalDisposable = Observable.Interval(TimeSpan.FromSeconds(waitTime)).Subscribe(_ =>
            {
                if (i <= repeatCount)
                {
                    action?.Invoke(i);
                    i++;
                }
                else
                {
                    finishCycleAction?.Invoke();
                    intervalDisposable.Dispose();
                }
                
            }).AddTo(monoObject);

            return intervalDisposable;
        }
    }

    public class Vector2XZ
    {
        public float x;
        public float z;

        public static implicit operator Vector2XZ(Vector3 vector3)
        {
            return new Vector2XZ()
            {
                x = vector3.x,
                z = vector3.z,
            };
        }

        public static implicit operator Vector2(Vector2XZ vector3)
        {
            return new Vector2()
            {
                x = vector3.x,
                y = vector3.z,
            };
        }
    }
}