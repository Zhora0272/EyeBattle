using System;
using Shop;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeTypeData", menuName = "Data/Shop/EyeTypeData")]
public class ShopEyeTypeScriptable : ScriptableObject
{
    [field: SerializeField] public BaseEyeTypeParameters[] TypeParameters { get; private set; }
}

[Serializable]
public class BaseEyeTypeParameters : BaseEyeItemParameters
{
    [field: SerializeField] public EyeType EyeType { get; private set; }
    [field: SerializeField] public Texture EyeTypeTexture { get; private set; }
    [field: SerializeField] public int Index { get; set; }
    [field: SerializeField] public ShopItemState ItemState { get; set; }
    [field: SerializeField] public BuyType BuyType { get; set; }
    public int PricePoint { get; set; }
}

public enum EyeType
{
    Human,
    Snake,
    Dragon,
}