using System;
using Shop;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeSizeData", menuName = "Data/Shop/EyeSizeData")]
public class ShopEyeSizeScriptable : ScriptableObject
{
    [field: SerializeField] public BaseEyeSizeParameters[] SizeParameters { get; private set; }
}

[Serializable]
public class BaseEyeSizeParameters : BaseEyeItemParameters
{
    [field: SerializeField] public float EyeSize { get; private set; }
    [field: SerializeField] public Texture EyeSizeTexture { get; private set; }
}