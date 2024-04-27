using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeDecorData", menuName = "Data/Shop/EyeDecorData")]
public class ShopEyeDecorScriptable : ScriptableObject
{
    [field:SerializeField] public BaseEyeDecorParameters[] DecorParameters { get; private set; }
}

[Serializable]
public class BaseEyeDecorParameters : BaseEyeItemParameters
{
    [field: SerializeField] public Texture EyeDecorTexture { get; private set; }
    [field: SerializeField] public GameObject DecorObject { get; private set; } 
}