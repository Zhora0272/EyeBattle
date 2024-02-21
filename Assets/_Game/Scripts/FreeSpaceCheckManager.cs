using UnityEngine;

public static class FreeSpaceCheckManager
{
    /// <summary>
    /// free space checker
    /// </summary>
    /// <param name="position"> check position</param>
    /// <param name="checkSize"> CheckBox size </param>
    /// <param name="layer">layerMask int value</param>
    /// <returns></returns>
    public static bool CheckVector(Vector3 position, float checkSize, int layer)
    {
        return Physics.CheckBox(
            position,
            Vector3.one * checkSize,
            Quaternion.identity, layer);
    }
}