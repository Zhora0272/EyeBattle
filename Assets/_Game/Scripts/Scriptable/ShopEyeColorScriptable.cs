using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopEyeColorData", menuName = "Data/Shop/EyeColorData")]
public class ShopEyeColorScriptable : ScriptableObject
{
    [field: SerializeField] public EyeColorParameters[] Colors { get; private set; }   
}

[Serializable]
public class EyeColorParameters
{
    [field: SerializeField] public Color Colors { get; private set; }
}

public interface ShopItemParameters
{
    public int Index { get; set; }
    public int Price { get; set; }
    
}
