using System;
using UnityEngine;

namespace Shop
{
    public class ShopEyeSizeView : ShopViewBase
    {
        enum EyeSizeType
        {
            EyeSize,
            EyeBibeSize,
        }
        
        [Header("Data")]
        [SerializeField] private ShopEyeSizeScriptable _eyeSizeScriptable;
        
        [SerializeField] private EyeSizeType _eyeColorType;
        
        protected override void InitData(int[] saveIndex)
        {
            
        }

        protected override void Init()
        {
            for (int i = 0; i < 3; i++)
            {
                var configs = _eyeSizeScriptable.SizeParameters[i];
                var value = configs.EyeSize;
                var item = Instantiate(_prefabRectTransform, _deactiavtedContent);

                //
                item.SetRaycastState(false);
                item.HideItemElements();
                item.SetValue(value);
                //
                
                _shopEyeItems.Add(item);
            }
        
            foreach (var configs in _eyeSizeScriptable.SizeParameters)
            {
                var item = Instantiate(_prefabRectTransform, _actiavtedContent);
                var value = configs.EyeSize;
                
                //
                item.SetBuyParameters(configs.BuyType, configs.PricePoint);
                item.SetValue(value);
                item.SetValueAction(SizeSelectAction);
                //
            }   
        }
        
        private void SizeSelectAction(float value)
        {
            EyeCustomizeModel item = null;

            switch (_eyeColorType)
            {
                case EyeSizeType.EyeSize:
                    item = new EyeCustomizeModel(eyeSize: value); 
                    break; 
                case EyeSizeType.EyeBibeSize:
                    item = new EyeCustomizeModel(eyeBibeSize: value); 
                    break;
            }
            
            _manager.CallBack.Value = item;
        }
    }
}