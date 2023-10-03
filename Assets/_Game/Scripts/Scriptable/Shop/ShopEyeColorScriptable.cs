using System;
using Shop;
using UnityEngine;

namespace Shop
{
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
        [field: SerializeField] public BuyType BuyType { get; set; }
        [field: SerializeField] public int PricePoint { get; set; }
    }
}