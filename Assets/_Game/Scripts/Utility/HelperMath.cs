using System;
using UnityEngine;
using System.Collections;
using Object = System.Object;
using Random = UnityEngine.Random;

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

        float randomXClamp = GetNumberSign(randomX) * clampDistance + randomX;
        float randomYClamp = GetNumberSign(randomY) * clampDistance + randomY;
        float randomZClamp = GetNumberSign(randomZ) * clampDistance + randomZ;

        return new Vector3(
            randomXClamp,
            ignoreY ? 0 : randomY,
            randomZClamp);
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

public static class CoroutineHelper
{
    private static IEnumerator WaitToObjectInit(MonoBehaviour obj, Action actionAfterInit)
    {
        yield return new WaitUntil(() => obj == null);
        actionAfterInit.Invoke();
    }

    public static void WaitToObjectInitAndDo
    (
        this MonoBehaviour monoObject,
        MonoBehaviour checkObject,
        Action doAfterInit
    )
    {
        monoObject.StartCoroutine(WaitToObjectInit(checkObject, doAfterInit));
    }

    private static void CoroutineDebug(string debug)
    {
        Debug.Log("CoroutineHelper:> " + debug);
    }
}