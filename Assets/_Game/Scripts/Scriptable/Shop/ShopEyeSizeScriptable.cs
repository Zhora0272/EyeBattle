using System;
using Shop;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeSizeData", menuName = "Data/Shop/EyeSizeData")]
public class ShopEyeSizeScriptable : ScriptableObject
{
    [field: SerializeField] public EyeSizeParameters[] SizeParameters { get; private set; }
}

[Serializable]
public class EyeSizeParameters : IEyeItemParameters
{
    [field: SerializeField] public float EyeSize { get; private set; }
    [field: SerializeField] public Texture EyeSizeTexture { get; private set; }

    [field: SerializeField] public int Index { get; set; }
    [field: SerializeField] public ShopItemState ItemState { get; set; }
    [field: SerializeField] public int PricePoint { get; set; }
    [field: SerializeField] public BuyType BuyType { get; set; }
}