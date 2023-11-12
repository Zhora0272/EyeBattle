using System.Collections.Generic;
using System.Linq;
using Saveing;
using UniRx;
using UnityEngine;

namespace Shop
{
    public class ShopViewBase : MonoBehaviour, IEyeItemCollectionSaveable
    {
        [SerializeField] protected ShopEyeItem _prefabRectTransform;
        [SerializeField] protected RectTransform _actiavtedContent;
        [SerializeField] protected RectTransform _deactiavtedContent;

        public ReactiveProperty<int> SelectedIndex = new(0);

        protected List<ShopEyeItem> _shopEyeItems = new();

        protected IManager<ShopCustomizeManager, EyeCustomizeModel> _manager;
        protected EyeItemCollection ItemData;


        public void SetManager(IManager<ShopCustomizeManager, EyeCustomizeModel> manager) =>
            _manager = manager;

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            var eyeItemsLenght = ItemData.BaseEyeItems.Length;

            for (int i = 0; i < eyeItemsLenght; i++)
            {
                _shopEyeItems[i].SetSelectedReactiveProperty(SelectedIndex);
                _shopEyeItems[i].SetData(ItemData.BaseEyeItems[i]);
            }
        }

        public void SetData((int, EyeItemCollection) data)
        {
            ItemData = data.Item2;
            SelectedIndex.Value = data.Item1;
        }

        public (int, EyeItemCollection) GetData()
        {
            List<BaseEyeItemParameters> baseEyeItemParameters = 
                _shopEyeItems.Select(item =>
                    (BaseEyeItemParameters) item).ToList();

            return new()
            {
                Item2 = new EyeItemCollection()
                {
                    BaseEyeItems = baseEyeItemParameters.ToArray()
                },
                Item1 = SelectedIndex.Value
            };
        }

        protected virtual void Init()
        {
        }
    }
}