using UnityEngine;

namespace Shop
{
    public class ShopEyeDecorView : ShopViewBase
    {
        enum EyeDecorType
        {
            EyeHeadDecor,
            EyeBodyDecor,
        }

        [Header("Data")] [SerializeField] private ShopEyeDecorScriptable _eyeDecorScriptable;
        [SerializeField] private EyeDecorType _eyeDecorType;

        protected override void Init()
        {
            base.Init();
            for (int i = 0; i < 3; i++)
            {
                var configs = _eyeDecorScriptable.Decors[i];
                var item = Instantiate(_prefabRectTransform, _deactiavtedContent);

                //
                item.SetRaycastState(false);
                item.HideItemElements();
                //
            }
        }

        private void DecorSelectAction(int index)
        {
            EyeCustomizeModel item = null;

            switch (_eyeDecorType)
            {
                case EyeDecorType.EyeHeadDecor:
                    item = new EyeCustomizeModel(eyeHeadDecor: index);
                    break;
                case EyeDecorType.EyeBodyDecor:
                    item = new EyeCustomizeModel(eyeBodyDecor: index);
                    break;
            }

            _manager.CallBack.Value = item;
        }

    }
}