using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeSizeData", menuName = "Data/Shop/EyeSizeData")]
public class ShopEyeSizeScriptable : ScriptableObject
{
    public EyeSizeParameters[] SizeParameters { get; private set; }
}

[Serializable]
public class EyeSizeParameters
{
    [field:SerializeField] public float EyeSize { get; private set; }
    [field:SerializeField] public Texture EyeSizeTexture { get; private set; }
}
