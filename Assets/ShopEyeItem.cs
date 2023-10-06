using System;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI _priceText;

        [Header("Item_State")] 
        [SerializeField] private GameObject _selectedState;
        [SerializeField] private GameObject _buyState;

        [SerializeField] private GameObject _itemElements;

        private FinanceManager _financeManager;

        private int _pricePoint;
        
        //item data variables
        private float _value;

        private void Awake()
        {
            _selectButton.onClick.AddListener(BuyAction);
        }

        private void Start()
        {
            _financeManager = MainManager.GetManager<FinanceManager>();
        }

        internal void SetBuyParameters(BuyType type, int pricePoint)
        {
            _buyType = type;
            _pricePoint = pricePoint;

            this.WaitToObjectInitAndDo(_financeManager, RefreshPrice);
        }
        
        private void RefreshPrice()
        {
            switch (_buyType)
            {
                case BuyType.Ads:
                    _priceText.text = _financeManager.ConvertPricePointTo(BuyType.Ads, _pricePoint) + "Ad";
                    break;
                case BuyType.Money:
                    _priceText.text = _financeManager.ConvertPricePointTo(BuyType.Money, _pricePoint) + "$";
                    break;
                case BuyType.Gem:
                    _priceText.text = _financeManager.ConvertPricePointTo(BuyType.Gem, _pricePoint) + "@";
                    break;
            }
        }

        internal void SetValue(float value) => _value = value;
        internal void SetColor(Color color) => _previewImage.color = color;
        internal void SetTexture(Texture texture) => _previewImage.texture = texture;
        internal void SetRaycastState(bool state) => _buttonImage.raycastTarget = state;

        #region SetConfigurations
        internal void SetColorAction(Action<Color> action)
        {
            _selectButton.onClick.AddListener(() => { action.Invoke(_previewImage.color); });
        }
        internal void SetValueAction(Action<float> action)
        {
            _selectButton.onClick.AddListener(() => { action.Invoke(_value); });
        }
        internal void SetTextureAction(Action<Color> action)
        {
            _selectButton.onClick.AddListener(() => { action.Invoke(_previewImage.color); });
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
                case ShopItemState.Selected:
                    _selectedState.SetActive(false);
                    break;
                case ShopItemState.Sale:
                    _buyState.SetActive(false);
                    break;
            }
        }
        #endregion

        internal void HideItemElements()
        {
            _itemElements.SetActive(false);
        }

        private void BuyAction()
        {
            _financeManager.TryBuy(_buyType, _pricePoint, BuyActionCallBack);
        }

        private void BuyActionCallBack(bool state)
        {
        }
    }
}