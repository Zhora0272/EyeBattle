using UnityEngine;

namespace Shop
{
    public class ShopEyeColorView : ShopViewBase
    {
        enum EyeColorType
        {
            EyeFront,
            EyeBack,
        }

        [Header("Data")]
        [SerializeField] private ShopEyeColorScriptable _eyeColorScriptable;

        [SerializeField] private EyeColorType _eyeColorType;
        
        protected override void InitData(int[] saveIndex)
        {
            
        }

        protected override void Init()
        {
            for (int i = 0; i < 3; i++)
            {
                var configs = _eyeColorScriptable.Colors[i];
                var color = configs.Colors;
                var item = Instantiate(_prefabRectTransform, _deactiavtedContent);

                item.SetRaycastState(false);

                _shopEyeItems.Add(item);
                
                item.SetColor(Helpers.AlphaToMax(color));
            }
        
            foreach (var configs in _eyeColorScriptable.Colors)
            {
                var item = Instantiate(_prefabRectTransform, _actiavtedContent);                
                var color = configs.Colors;

                item.SetBuyType(configs.BuyType);
                item.SetColor(Helpers.AlphaToMax(color));
                item.SetColorAction(ColorSelectAction);
            }   
        }

        private void ColorSelectAction(Color color)
        {
            EyeCustomizeModel item = null;

            switch (_eyeColorType)
            {
                case EyeColorType.EyeFront:
                    item = new EyeCustomizeModel(eyeColor: color); 
                    break; 
                case EyeColorType.EyeBack:
                    item = new EyeCustomizeModel(eyeBackColor: color); 
                    break;
            }

            _manager.CallBack.Value = item;
        }
    }
}
