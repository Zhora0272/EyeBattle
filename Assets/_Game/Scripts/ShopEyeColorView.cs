using _Game.Scripts.Utility;
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
                
                DeactivatedContentInit(out var item);
                
                item.SetColor(HelperMath.AlphaToMax(color));
            }

            foreach (var configs in _eyeColorScriptable.Colors)
            {
                ActivatedContentInit(out var item, configs);
                
                var color = configs.Color;

                item.SetColor(HelperMath.AlphaToMax(color));
                item.SetColorAction(ColorSelectAction);
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

            _manager.CallBack.Value = item;
        }
    }
}