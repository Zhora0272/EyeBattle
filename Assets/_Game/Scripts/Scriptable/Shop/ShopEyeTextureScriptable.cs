using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ShopEyeTextureData", menuName = "Data/Shop/EyeTextureData")]
public class ShopEyeTextureScriptable : ScriptableObject
{
    [field: SerializeField] public BaseEyeTextureParameters[] TextureParameters { get; private set; }
}

[Serializable]
public class BaseEyeTextureParameters : BaseEyeItemParameters
{
    
}