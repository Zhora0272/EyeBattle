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
                
                item.SetColor(new Color(color.r, color.b, color.g,1));
            }
        
            foreach (var configs in _eyeColorScriptable.Colors)
            {
                var item = Instantiate(_prefabRectTransform, _actiavtedContent);
                
                var color = configs.Colors;
                item.SetColor(new Color(color.r, color.b, color.g,1));
                item.SetManager(ColorSelectAction);
            }   
        }

        private void ColorSelectAction(Color color)
        {
            
        }
    }
}
