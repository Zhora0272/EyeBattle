using System;
using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "ShopEyeColorData", menuName = "Data/Shop/EyeColorData")]
    public class ShopEyeColorScriptable : ScriptableObject
    {
        [field: SerializeField] public BaseEyeColorParameters[] Colors { get; private set; }
    }

    [Serializable]
    public class BaseEyeColorParameters : BaseEyeItemParameters
    {
        [field: SerializeField] public Color Color { get; private set; }
    }
}