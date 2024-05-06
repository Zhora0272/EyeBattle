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
        [SerializeField] private EyeSizeType _eyeSizeType;

        protected override void Init()
        {
            base.Init();
            for (int i = 0; i < 3; i++)
            {
                var configs = _eyeSizeScriptable.SizeParameters[i];
                var value = configs.EyeSize;
                
                DeactivatedContentInit(out var item);
                
                item.SetValue(value);
            }

            foreach (var configs in _eyeSizeScriptable.SizeParameters)
            {
                ActivatedContentInit(out var item, configs);
                
                var value = configs.EyeSize;
                
                item.SetValue(value);
                item.SelectAction(SizeSelectAction);
            }
        }

        private void SizeSelectAction(int index)
        {
            EyeCustomizeModel item = null;

            switch (_eyeSizeType)
            {
                /*case EyeSizeType.EyeSize:
                    item = new EyeCustomizeModel(eyeSize: index);
                    break;*/
                case EyeSizeType.EyeBibeSize:
                    item = new EyeCustomizeModel(eyePupilSize: index);
                    break;
            }

            _manager.CallBack.Value = item;
        }
    }
}