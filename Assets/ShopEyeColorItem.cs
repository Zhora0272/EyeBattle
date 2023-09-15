using System;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopEyeColorItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;

        [SerializeField] private GameObject _selectedState;
        [SerializeField] private GameObject _buyState;
        
        internal void SetColor(Color color) => _image.color = color;
    
        internal void SetManager(Action<Color> action)
        {
            _button.onClick.AddListener(() =>
            {
                action.Invoke(_image.color);
            });
        }

        internal void SetState(ShopItemState state)
        {
            switch (state)
            {
                case ShopItemState.Empty:
                {
                    _selectedState.SetActive(false);
                    _buyState.SetActive(false);
                    break;
                }
                case ShopItemState.Selected : _selectedState.SetActive(false); break;
                case ShopItemState.Sale : _buyState.SetActive(false); break;
            }
        }
    }

    enum ShopItemState
    {
        Empty,
        Selected,
        Sale,
    }
}

