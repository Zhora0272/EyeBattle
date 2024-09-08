using UnityEngine;

public struct ScreenSelectRangePoint
{
    public Vector2 SelectionStartVector;
    public Vector2 SelectionEndVector;

    public ScreenSelectRangePoint(Vector2 startVector, Vector2 endVector)
    {
        SelectionStartVector = startVector;
        SelectionEndVector = endVector;
    }
}