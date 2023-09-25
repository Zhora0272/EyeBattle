using UnityEngine;
using System;
using Shop;

[CreateAssetMenu(fileName = "ShopEyeTextureData", menuName = "Data/Shop/EyeTextureData")]
public class ShopEyeTextureScriptable : ScriptableObject
{
    [field: SerializeField] public EyeTextureParameters[] TextureParameters { get; private set; }
}

[Serializable]
public class EyeTextureParameters : IEyeItemParameters
{
    [field: SerializeField] public int Index { get; set; }
    [field: SerializeField] public ShopItemState ItemState { get; set; }
}