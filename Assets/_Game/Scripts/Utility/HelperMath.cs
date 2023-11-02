using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public static class HelperMath
{
    public static int LenghtToCount(int value)
    {
        return (value - 1);
    }

    public static Vector3 GetRandomPosition(float min, float max)
    {
        return new Vector3(
            Random.Range(min, max),
            Random.Range(min, max),
            Random.Range(min, max));
    }

    public static int ClampMin(int value, int minValue) =>
        value < minValue ? minValue : value;

    public static int ClampMax(int value, int maxValue) =>
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
    public static IEnumerator WaitToObjectInit(MonoBehaviour obj, Action actionAfterInit)
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