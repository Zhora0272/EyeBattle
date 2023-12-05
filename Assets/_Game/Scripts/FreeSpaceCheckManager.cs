using UnityEngine;

public static class FreeSpaceCheckManager
{
    public static bool CheckVector(Vector3 position)
    {
        return Physics.CheckBox(position, Vector3.one, Quaternion.identity, LayerMask.NameToLayer("Ground"));
    }
}