using UnityEngine;

namespace Shop
{
    public class ShopEyeTextureView : ShopViewBase
    {
        [Header("Data")]
        [SerializeField] private ShopEyeTextureScriptable _eyeTextureScriptable;

        protected override void Init()
        {
            base.Init();
            for (int i = 0; i < 3; i++)
            {
                var configs = _eyeTextureScriptable.TextureParameters[i];
                var texture = configs.Texture;
                
                DeactivatedContentInit(out var item);
                
                item.SetColor(Color.white);
                item.SetTexture(texture);
            }
            
            foreach (var configs in _eyeTextureScriptable.TextureParameters)
            {
                ActivatedContentInit(out var item, configs);
                
                var texture = configs.Texture;
                
                item.SetTexture(texture);
                item.SetColor(Color.white);
                
                item.SelectAction(TextureSelectAction);
            }
        }
        private void TextureSelectAction(int index)
        {
            _manager.CallBack.Value = new EyeCustomizeModel(eyeType: index);;
        }
    }
}
