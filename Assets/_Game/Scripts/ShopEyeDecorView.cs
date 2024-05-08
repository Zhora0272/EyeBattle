using UnityEngine;

namespace Shop
{
    public class ShopEyeDecorView : ShopViewBase
    {
        [Header("Data")] [SerializeField] private ShopEyeDecorScriptable _eyeDecorScriptable;

        protected override void Init()
        {
            base.Init();
            for (int i = 0; i < 3; i++)
            {
                var configs = _eyeDecorScriptable.DecorParameters[i];
                
                DeactivatedContentInit(out var item);

                item.SetTexture(configs.EyeDecorTexture);
                item.SelectAction(DecorSelectAction);
                item.SetColor(Color.white);
            }
            
            foreach (var configs in _eyeDecorScriptable.DecorParameters)
            {
                ActivatedContentInit(out var item, configs);

                item.SetTexture(configs.EyeDecorTexture);
                item.SelectAction(DecorSelectAction);
                item.SetColor(Color.white);
            }
        }

        private void DecorSelectAction(int index)
        {
            EyeCustomizeModel item = null;

            item = new EyeCustomizeModel(eyeDecor: index);
            
            _manager.CallBack.Value = item;
        }

    }
}