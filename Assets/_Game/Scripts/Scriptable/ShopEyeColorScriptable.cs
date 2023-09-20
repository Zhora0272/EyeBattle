using System;
using Shop;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeColorData", menuName = "Data/Shop/EyeColorData")]
public class ShopEyeColorScriptable : ScriptableObject
{
    [field: SerializeField] public EyeColorParameters[] Colors { get; private set; }
}

[Serializable]
public class EyeColorParameters : IEyeItemParameters
{
    [field: SerializeField] public Color Colors { get; private set; }
    [field: SerializeField] public int Index { get; set; }
    [field: SerializeField] public ShopItemState ItemState { get; set; }
}

public interface ShopItemParameters
{
    public int Index { get; set; }
    public int Price { get; set; }
}