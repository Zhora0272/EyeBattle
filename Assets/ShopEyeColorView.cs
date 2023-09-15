using UnityEngine;
using UnityEngine.UI;

public class ShopEyeColorView : MonoBehaviour
{
    [SerializeField] private ShopEyeColorScriptable _eyeColorScriptable;
    [SerializeField] private RectTransform _prefabRectTransform;
    [SerializeField] private RectTransform _content;

    private void Awake()
    {
        _prefabRectTransform = GetComponent<RectTransform>();
    }
    
    private void Start()
    {
        foreach (var color in _eyeColorScriptable.Colors)
        {
            var item = Instantiate(_prefabRectTransform, _content);
            item.GetComponent<Image>().color = color;
        }
    }
}
