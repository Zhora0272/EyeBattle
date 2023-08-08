using System.Collections.Generic;
using UnityEngine;

public enum ShopParameters
{
    ShopSelectedTheme,
}

public class ShopController : MonoBehaviour
{
    [SerializeField] private WorldChangeController _worldChangeController;
    [SerializeField] private ShopThemeItem _itemPrefab;
    [SerializeField] private Data2048[] _data;

    [SerializeField] private Transform _content;

    private List<ShopThemeItem> _elements;

    private void Awake()
    {
        _elements = new List<ShopThemeItem>();
    }

    private void Start()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            var item = Instantiate(_itemPrefab, _content);
            item.SetImage(_data[i].WorldScreen);
            item.SetClickCallBackAction(SelectDeselectItems);
            _elements.Add(item);
        }

        SelectDeselectItems(PlayerPrefs.GetInt(ShopParameters.ShopSelectedTheme.ToString()));
    }


    private void SelectDeselectItems(int index)
    {
        foreach (var item in _elements)
        {
            var state = item.transform.GetSiblingIndex() == index;

            if(!state)
            {
                item.DeselectItem();
            }
            else
            {
                PlayerPrefs.SetInt(ShopParameters.ShopSelectedTheme.ToString(), index);
            }
        }
    }
}
