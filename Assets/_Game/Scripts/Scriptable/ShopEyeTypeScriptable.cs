using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeTypeData", menuName = "Data/Shop/EyeTypeData")]
public class ShopEyeTypeScriptable : ScriptableObject
{
    [field:SerializeField] public EyeTypeParameters[] TypeParameters { get; private set; }
}

[Serializable]
public class EyeTypeParameters
{
    [field:SerializeField] public float EyeType { get; private set; }
    [field:SerializeField] public Texture EyeTypeTexture { get; private set; }
}
