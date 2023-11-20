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


        protected override void Init()
        {
            base.Init();
            for (int i = 0; i < 3; i++)
            {
                var configs = _eyeColorScriptable.Colors[i];
                var color = configs.Color;
                var item = Instantiate(_prefabRectTransform, _deactiavtedContent);

                //
                item.SetRaycastState(false);
                item.HideItemElements();
                item.SetColor(HelperMath.AlphaToMax(color));
                //
            }

            foreach (var configs in _eyeColorScriptable.Colors)
            {
                var item = Instantiate(_prefabRectTransform, _actiavtedContent);
                var color = configs.Color;

                item.SetData(configs);
                item.SetColor(HelperMath.AlphaToMax(color));
                item.SetColorAction(ColorSelectAction);
                
                _shopEyeItems.Add(item);
            }
        }

        private void ColorSelectAction(int index)
        {
            EyeCustomizeModel item = null;

            switch (_eyeColorType)
            {
                case EyeColorType.EyeFront:
                    item = new EyeCustomizeModel(eyeColor: index);
                    break;
                case EyeColorType.EyeBack:
                    item = new EyeCustomizeModel(eyeBackColor: index);
                    break;
            }

            
            this.WaitToObjectInitAndDo(_manager, () =>
            {
                _manager.CallBack.Value = item;
            });
        }
    }
}