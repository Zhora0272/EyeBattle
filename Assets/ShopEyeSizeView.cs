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
                var configs = _eyeSizeScriptable.SizeParamete[i];
                var value = configs.EyeSize;
                var item = Instantiate(_prefabRectTransform, _deactiavtedContent);

                item.SetRaycastState(false);

                _shopEyeItems.Add(item);
                
                item.SetValue(value);
            }
        
            foreach (var configs in _eyeSizeScriptable.SizeParamete)
            {
                var item = Instantiate(_prefabRectTransform, _actiavtedContent);                
                var value = configs.EyeSize;

                item.SetValueAction(ColorSelectAction);
            }   
        }
        
        private void ColorSelectAction(float value)
        {
            EyeCustomizeModel item = null;

            switch (_eyeColorType)
            {
                case EyeSizeType.EyeSize:
                    item = new EyeCustomizeModel(eyeSize: value); 
                    break; 
                /*case EyeSizeType.EyeBibeSize:
                    item = new EyeCustomizeModel(eyeBackColor: color); 
                    break;*/
            }

            _manager.CallBack.Value = item;
        }
    }
}