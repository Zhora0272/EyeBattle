using _Project.Scripts.Utilities;
using UnityEngine;

namespace Shop
{
    public class ShopEyeColorView : ShopViewBase
    {
        [Header("Data")]
        [SerializeField] private ShopEyeColorScriptable _eyeColorScriptable;
        
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
                
                _shopEyeItems.Add(item);
                
                item.SetColor(Helpers.AlphaToMax(color));
            }
        
            foreach (var configs in _eyeColorScriptable.Colors)
            {
                var item = Instantiate(_prefabRectTransform, _actiavtedContent);
                
                var color = configs.Colors;

                item.SetColor(Helpers.AlphaToMax(color));
                
                item.SetManager(ColorSelectAction);
            }   
        }

        private void ColorSelectAction(Color color)
        {
            
        }
    }
}
