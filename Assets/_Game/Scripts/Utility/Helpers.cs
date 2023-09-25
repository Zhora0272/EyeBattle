using System;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Helpers
{
    public static Vector3 GetRandomPosition(float min, float max)
    {
        return new Vector3(
            Random.Range(min, max),
            Random.Range(min, max),
            Random.Range(min, max));
    }
        
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
        return Vector2.Distance( a, b );
    }
        
    public static bool Equals(double x, double y, double tolerance)
    {
        var diff = Math.Abs(x - y);
        return diff <= tolerance ||
               diff <= Math.Max(Math.Abs(x), Math.Abs(y)) * tolerance;
    }
}