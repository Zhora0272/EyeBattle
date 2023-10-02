using System;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopEyeItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _buttonImage;

        [SerializeField] private RawImage _image;

        [SerializeField] private GameObject _selectedState;
        [SerializeField] private GameObject _buyState;

        private float _value;

        internal void SetValue(float value) => _value = value; 
        internal void SetColor(Color color) => _image.color = color;
        internal void SetTexture(Texture texture) => _image.texture = texture;

        internal void SetRaycastState(bool state)
        {
            _buttonImage.raycastTarget = state;
        }
    
        internal void SetColorAction(Action<Color> action)
        {
            _button.onClick.AddListener(() =>
            {
                action.Invoke(_image.color);
            });
        }
        
        internal void SetValueAction(Action<float> action)
        {
            _button.onClick.AddListener(() =>
            {
                action.Invoke(_value);
            });
        }
        
        internal void SetTextureAction(Action<Color> action)
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
}

