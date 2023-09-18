using UnityEngine;

namespace Shop
{
    public class ShopEyeTypeView : ShopViewBase
    {
        [Header("Data")]
        [SerializeField] private ShopEyeTypeScriptable _eyeTypeScriptable;
        
        protected override void Init()
        {
            for (int i = 0; i < 3; i++)
            {
                var texture = _eyeTypeScriptable.TypeParameters[i].EyeTypeTexture;
                var item = Instantiate(_prefabRectTransform, _deactiavtedContent);
                item.SetTexture(texture);
            }
        
            foreach (var config in _eyeTypeScriptable.TypeParameters)
            {
                var item = Instantiate(_prefabRectTransform, _actiavtedContent);
                item.SetTexture(config.EyeTypeTexture);
                item.SetManager(TextureSelectAction);
            }   
        }
        
        private void TextureSelectAction(Color color)
        {
            
        }
    }
}
