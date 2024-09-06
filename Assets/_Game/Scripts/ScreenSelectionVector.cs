using UnityEngine;

public struct ScreenSelectionVector
{
    public Vector2 SelectionStartVector;
    public Vector2 SelectionEndVector;

    public ScreenSelectionVector(Vector2 startVector, Vector2 endVector)
    {
        SelectionStartVector = startVector;
        SelectionEndVector = endVector;
    }
}