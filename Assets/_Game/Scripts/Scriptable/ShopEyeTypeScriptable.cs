using System;
using Shop;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeTypeData", menuName = "Data/Shop/EyeTypeData")]
public class ShopEyeTypeScriptable : ScriptableObject
{
    [field:SerializeField] public EyeTypeParameters[] TypeParameters { get; private set; }
}

[Serializable]
public class EyeTypeParameters : IEyeItemParameters
{
    [field:SerializeField] public EyeType EyeType { get; private set; }
    [field:SerializeField] public Texture EyeTypeTexture { get; private set; }
    [field:SerializeField] public int Index { get; set; }
    [field:SerializeField] public ShopItemState ItemState { get; set; }
}

public enum EyeType
{
    Human,
    Snake,
    Dragon,
}