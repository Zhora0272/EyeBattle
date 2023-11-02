using System;
using Saveing;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Shop
{
    public class ShopEyeItem : MonoBehaviour, IEyeBaseItemParametersSaveable
    {
        [SerializeField] private BuyType _buyType;
        [SerializeField] private ReactiveProperty<ShopItemState> _itemState = new(ShopItemState.Sale);

        [Header("Parameters")] [SerializeField]
        private Button _selectButton;

        [SerializeField] private Image _buttonImage;
        [SerializeField] private RawImage _previewImage;
        [SerializeField] private TextMeshProUGUI _priceText;

        [Header("Item_State")] [SerializeField]
        private GameObject _selectedState;

        [SerializeField] private GameObject _buyState;
        [SerializeField] private GameObject _itemElements;

        private ReactiveProperty<int> _selectedIndex;

        private FinanceManager _financeManager;
        private DataManager _dataManager;
        
        private Action<bool> _onClickEvent;

        //item data variables
        private int _indexInQueue;
        private int _pricePoint;
        private int _colorIndex;

        private float _value;
        //

        public static implicit operator BaseEyeItemParameters(ShopEyeItem data)
        {
            return new BaseEyeItemParameters()
            {
                Index = data._indexInQueue,
                PricePoint = data._pricePoint,
                BuyType = data._buyType,
                ItemState = data._itemState.Value,
            };
        }

        private void Awake()
        {
            _selectButton.onClick.AddListener(ClickAction);

            _itemState.Subscribe(SetState).AddTo(this);
        }

        private void Start()
        {
            _financeManager = MainManager.GetManager<FinanceManager>();
            _dataManager = MainManager.GetManager<DataManager>();
        }

        internal void SetValue(float value) => _value = value;
        internal void SetColor(Color color) => _previewImage.color = color;
        internal void SetTexture(Texture texture) => _previewImage.texture = texture;
        internal void SetRaycastState(bool state) => _buttonImage.raycastTarget = state;

        #region SetConfigurations

        internal void SetColorAction(Action<int> action)
        {
            _onClickEvent = state =>
            {
                if (state)
                {
                    action.Invoke(_colorIndex);
                }
            };
        }
        internal void SetValueAction(Action<float> action)
        {
            _onClickEvent = state =>
            {
                if (state)
                {
                    action.Invoke(_value);
                }
            };
        }
        internal void SetTextureAction(Action<Texture> action)
        {
            _onClickEvent = state =>
            {
                if (state)
                {
                    action.Invoke(_previewImage.texture);
                }
            };
        }

        private void SetState(ShopItemState state)
        {
            if (state == ShopItemState.Empty)
            {
                _buyState.SetActive(false);
                _selectedState.SetActive(false);
            }
            else
            {
                _selectedState.SetActive(state == ShopItemState.Selected);
                _buyState.SetActive(state == ShopItemState.Sale);
            }
        }

        #endregion

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
                    _priceText.text = _financeManager.ConvertPricePointTo(BuyType.Gem, _pricePoint) + "#";
                    break;
            }
        }

        internal void HideItemElements()
        {
            _itemElements.SetActive(false);
        }


        private void ClickAction()
        {
            switch (_itemState.Value)
            {
                case ShopItemState.Sale:
                    _financeManager.TryBuy(_buyType, _pricePoint, BuyActionCallBack);
                    break;
                case ShopItemState.Empty:
                    _itemState.Value = ShopItemState.Selected;
                    break;
            }
        }

        private void BuyActionCallBack(bool state)
        {
            if (state)
            {
                _selectedIndex.Value = _indexInQueue;
                _itemState.Value = ShopItemState.Selected;
            }
            else
            {
                _itemState.Value = ShopItemState.Sale;
            }

            _onClickEvent.Invoke(state);
        }

        public void SetData(BaseEyeItemParameters data)
        {
            _buyType = data.BuyType;
            _pricePoint = data.PricePoint;
            _itemState.Value = data.ItemState;
            _indexInQueue = data.Index;

            this.WaitToObjectInitAndDo(_financeManager, RefreshPrice);
        }
        public void SetSelectedReactiveProperty(ReactiveProperty<int> selectedIndex)
        {
            _selectedIndex = selectedIndex;

            _selectedIndex.Subscribe(value =>
            {
                if (_itemState.Value != ShopItemState.Sale)
                {
                    _itemState.Value = value == _indexInQueue ? 
                        ShopItemState.Selected : ShopItemState.Empty;
                }
            }).AddTo(this);
        }

        BaseEyeItemParameters ISaveable<BaseEyeItemParameters>.GetData()
        {
            return new()
            {
                PricePoint = _pricePoint,
                BuyType = _buyType,
                ItemState = _itemState.Value,
            };
        }
    }
}