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
        private readonly ReactiveProperty<ShopItemState> _itemState = new();
        private IReactiveProperty<ShopItemState> ItemState => _itemState;
        
        [SerializeField] private BuyType _buyType;

        [Header("Parameters")] [SerializeField]
        private Button _selectButton;

        [SerializeField] private Image _buttonImage;
        [SerializeField] private RawImage _previewImage;
        [SerializeField] private TextMeshProUGUI _priceText;

        [Header("Item_State")] [SerializeField]
        private GameObject _selectedState;

        [SerializeField] private GameObject _buyState;
        [SerializeField] private GameObject _itemElements;

        //MonoManager
        private FinanceManager _financeManager;

        //ReactiveProperty
        private ReactiveProperty<int> _selectedIndex = new();

        //Action
        private Action<bool> _selectButtonClickEvent;

        //item data variables
        private int _indexInQueue;
        private int _pricePoint;
        private int _colorIndex;

        private float _value;

        //Implicit
        public static implicit operator BaseEyeItemParameters(ShopEyeItem data)
        {
            return new BaseEyeItemParameters()
            {
                Index = data._indexInQueue,
                PricePoint = data._pricePoint,
                BuyType = data._buyType,
                ItemState = data.ItemState.Value,
            };
        }

        private void Awake()
        {
            _selectButton.onClick.AddListener(SelectButtonEvent);
            ItemState.Subscribe(SetState).AddTo(this);
        }

        private void Start()
        {
            _financeManager = MainManager.GetManager<FinanceManager>();
            Init();
        }

        private void Init()
        {
            RefreshPrice();
        }

        internal void SetRaycastState(bool state)
        {
            _buttonImage.raycastTarget = state;
            _selectButton.enabled = false;
            _selectButton.onClick.RemoveAllListeners();
        }

        internal void SetValue(float value) => _value = value;
        internal void SetColor(Color color) => _previewImage.color = color;
        internal void SetTexture(Texture texture) => _previewImage.texture = texture;

        #region SetEventActions

        internal void SelectAction(Action<int> action)
        {
            _selectButtonClickEvent = state =>
            {
                if (state)
                {
                    action.Invoke(_indexInQueue);
                }
            };
        }

        #endregion

        //Refactoring
        private void SetState(ShopItemState state)
        {
            _buyState.SetActive(state == ShopItemState.Sale);
            _selectedState.SetActive(state == ShopItemState.Selected);

            switch (state)
            {
                case ShopItemState.Selected:
                {
                    _selectedIndex.Value = _indexInQueue;
                    _selectButtonClickEvent?.Invoke(true);
                }
                    break;

                case ShopItemState.Empty:
                {
                }
                    break;

                case ShopItemState.Sale:
                {
                }
                    break;
            }
        }

        private void SelectButtonEvent()
        {
            switch (ItemState.Value)
            {
                case ShopItemState.Sale:
                {
                    _financeManager.TryBuy(_buyType, _pricePoint, TryBuyCallBack);
                }
                    break;
                case ShopItemState.Empty:
                {
                    ItemState.Value = ShopItemState.Selected;
                }
                    break;
            }
        }

        private void TryBuyCallBack(bool state, int pricePoint)
        {
            _pricePoint = pricePoint;

            if (state)
            {
                ItemState.Value = ShopItemState.Selected;
                _selectButtonClickEvent.Invoke(true);
            }
            else
            {
                ItemState.Value = ShopItemState.Sale;
            }

            RefreshPrice();
        }

        internal void HideItemElements()
        {
            _itemElements.SetActive(false);
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
                    _priceText.text = _financeManager.ConvertPricePointTo(BuyType.Gem, _pricePoint) + "#";
                    break;
            }
        }

        public void SetSelectedReactiveProperty(ReactiveProperty<int> selectedIndex)
        {
            _selectedIndex = selectedIndex;

            _selectedIndex.Subscribe(value =>
            {
                if (_itemState.Value != ShopItemState.Sale)
                {
                    ItemState.Value = value == _indexInQueue ?
                        ShopItemState.Selected :
                        ShopItemState.Empty;
                }
            }).AddTo(this);
        }

        public void SetData(BaseEyeItemParameters data)
        {
            _buyType = data.BuyType;
            _pricePoint = data.PricePoint;
            ItemState.Value = data.ItemState;
            _indexInQueue = data.Index;
        }

        BaseEyeItemParameters ISaveable<BaseEyeItemParameters>.GetData()
        {
            return new()
            {
                PricePoint = _pricePoint,
                BuyType = _buyType,
                ItemState = ItemState.Value,
            };
        }
    }
}