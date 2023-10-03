using System;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopEyeItem : MonoBehaviour
    {
        [SerializeField] private BuyType _buyType;
        
        [Header("Parameters")]
        [SerializeField] private Button _selectButton;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private RawImage _previewImage;

        [Header("Item_State")]
        [SerializeField] private GameObject _selectedState;
        [SerializeField] private GameObject _buyState;

        private int _adsCount = 10;
        private float _value;

        private void Awake()
        {
            _selectButton.onClick.AddListener(TryBuy);
        }

        internal void SetBuyType(BuyType type) => _buyType = type;
        internal void SetValue(float value) => _value = value; 
        internal void SetColor(Color color) => _previewImage.color = color;
        internal void SetTexture(Texture texture) => _previewImage.texture = texture;

        internal void SetRaycastState(bool state) =>  _buttonImage.raycastTarget = state;
        internal void SetColorAction(Action<Color> action)
        {
            _selectButton.onClick.AddListener(() =>
            {
                action.Invoke(_previewImage.color);
            });
        }
        internal void SetValueAction(Action<float> action)
        {
            _selectButton.onClick.AddListener(() =>
            {
                action.Invoke(_value);
            });
        }
        internal void SetTextureAction(Action<Color> action)
        {
            _selectButton.onClick.AddListener(() =>
            {
                action.Invoke(_previewImage.color);
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
        private void TryBuy()
        {
            MainManager.GetManager<FinanceManager>().TryBuy();
        }
    }
}

