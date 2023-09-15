using UnityEngine;

namespace Shop
{
    public class ShopEyeColorView : MonoBehaviour
    {
        [SerializeField] private ShopEyeColorScriptable _eyeColorScriptable;
        [SerializeField] private ShopEyeColorItem _prefabRectTransform;
        [SerializeField] private RectTransform _actiavtedContent;
        [SerializeField] private RectTransform _deactiavtedContent;

        private void Start()
        {
            for (int i = 0; i < 3; i++)
            {
                var color = _eyeColorScriptable.Colors[i];
                var item = Instantiate(_prefabRectTransform, _deactiavtedContent);
                item.SetColor(new Color(color.r, color.b, color.g,1));
            }
        
            foreach (var color in _eyeColorScriptable.Colors)
            {
                var item = Instantiate(_prefabRectTransform, _actiavtedContent);
                item.SetColor(new Color(color.r, color.b, color.g,1));
                item.SetManager(ColorSelectAction);
            }
        }

        private void ColorSelectAction(Color color)
        {
            
        }
    }
}
