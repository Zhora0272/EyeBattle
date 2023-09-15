using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeColorData", menuName = "Data/Shop/EyeColorData")]
public class ShopEyeColorScriptable : ScriptableObject
{
    [field: SerializeField] public Color[] Colors { get; private set; }
}
