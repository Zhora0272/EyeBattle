using System;
using Shop;

namespace Data
{
    public class DataManager : MonoManager
    {
        public ShopEyeSizeScriptable EyeSize;
        public ShopEyeSizeScriptable EyePupilSize;

        public ShopEyeColorScriptable EyeColor;
        public ShopEyeColorScriptable EyeBackColor;

        public ShopEyeTextureScriptable EyeTexture;
        
        public ShopEyeDecorScriptable EyeDecor;

        public EyeItemCollection[] GetAllDataLists()
        {
            return new EyeItemCollection[]
            {
                new() { BaseEyeItems = EyeColor.Colors },
                new() { BaseEyeItems = EyeBackColor.Colors },

                new() { BaseEyeItems = EyeTexture.TextureParameters },

                new() { BaseEyeItems = EyePupilSize.SizeParameters },
                
                new() { BaseEyeItems = EyeDecor.DecorParameters },
            };
        }
    }
}

[Serializable]
public class EyeItemCollection
{
    public BaseEyeItemParameters[] BaseEyeItems;
}