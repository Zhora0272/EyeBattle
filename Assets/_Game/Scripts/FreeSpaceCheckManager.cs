using UnityEngine;

public static class FreeSpaceCheckManager
{
    public static bool CheckVector(Vector3 position, float checkSize, int layer)
    {
        return Physics.CheckBox(
            position,
            Vector3.one * checkSize,
            Quaternion.identity, layer);
    }
}