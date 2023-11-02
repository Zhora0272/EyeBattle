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

        protected List<ShopEyeItem> _shopEyeItems = new();
        protected IManager<ShopCustomizeManager, EyeCustomizeModel> _manager;

        protected EyeItemCollection ItemData;

        protected ReactiveProperty<int> SelectedIndex = new(-1);

        public void SetManager(IManager<ShopCustomizeManager, EyeCustomizeModel> manager) => _manager = manager;

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            var lenght = ItemData.BaseEyeItems.Length;

            for (int j = 0; j < lenght; j++)
            {
                var eyeItemsLenght = ItemData.BaseEyeItems.Length;

                for (int i = 0; i < eyeItemsLenght; i++)
                {
                    _shopEyeItems[i].SetSelectedReactiveProperty(SelectedIndex);
                    _shopEyeItems[i].SetData(ItemData.BaseEyeItems[j]);
                }
            }
        }
        
        public void SetData(EyeItemCollection data)
        {
            ItemData = data;
        }
        
        EyeItemCollection ISaveable<EyeItemCollection>.GetData()
        {
            List<BaseEyeItemParameters> baseEyeItemParameters = new();

            baseEyeItemParameters.AddRange(_shopEyeItems.Cast<BaseEyeItemParameters>().ToArray());

            return new()
            {
                BaseEyeItems = baseEyeItemParameters.ToArray()
            };
        }
        
        protected virtual void Init()
        {
        }
    }
}