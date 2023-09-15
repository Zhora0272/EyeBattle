using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeSizeData", menuName = "Data/Shop/EyeSizeData")]
public class ShopEyeSizeScriptable : ScriptableObject
{
    [field:SerializeField] public float[] EyeSize { get; private set; }
}
