using System;
using Shop;

public class DataManager : MonoManager
{
    public ShopEyeColorScriptable EyeColor;
    public ShopEyeColorScriptable EyeBackColor;

    public EyeItemCollection[] GetAllDataLists()
    {
        return new EyeItemCollection[]
        {
            new() {BaseEyeItems = EyeColor.Colors},
            new() {BaseEyeItems = EyeBackColor.Colors},
        };
    }
}

[Serializable]
public class EyeItemCollection
{
    public BaseEyeItemParameters[] BaseEyeItems;
}