using UnityEngine;

public class ShopController : MonoBehaviour
{
    [SerializeField] private ShopThemeItem _itemPrefab;
    [SerializeField] private Data2048[] _data;

    [SerializeField] private Transform _content;


    private void Start()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            var item = Instantiate(_itemPrefab, _content);
            item.SetImage(_data[i].WorldScreen);
        }
    }
}
