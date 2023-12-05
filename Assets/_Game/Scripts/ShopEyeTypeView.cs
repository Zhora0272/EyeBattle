using UnityEngine;

namespace Shop
{
    public class ShopEyeTypeView : ShopViewBase
    {
        [Header("Data")]
        [SerializeField] private ShopEyeTypeScriptable _eyeTypeScriptable;
        
        protected override void Init()
        {
            base.Init();
            for (int i = 0; i < 3; i++)
            {
                var item = Instantiate(_prefabRectTransform, _deactiavtedContent);
                
                var texture = _eyeTypeScriptable.TypeParameters[i].EyeTypeTexture;
                
                SetEyeItemTextureAndColor(item, texture, Color.white);
            }
        
            foreach (var config in _eyeTypeScriptable.TypeParameters)
            {
                var item = Instantiate(_prefabRectTransform, _actiavtedContent);
                
                item.SetColorAction(TextureSelectAction);

                SetEyeItemTextureAndColor(item, config.EyeTypeTexture, Color.white);
            }   
        }

        private void SetEyeItemTextureAndColor(
            ShopEyeItem item,
            Texture texture,
            Color color
            )
        {
            item.SetTexture(texture);
            item.SetColor(color);
        }
        
        private void TextureSelectAction(int index) { }
    }
}
