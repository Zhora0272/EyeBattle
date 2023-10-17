using System.Collections.Generic;
using UnityEngine;

namespace Shop
{
    public class ShopViewBase : MonoBehaviour, ISaveable
    {
        [SerializeField] protected ShopEyeItem _prefabRectTransform;
        [SerializeField] protected RectTransform _actiavtedContent;
        [SerializeField] protected RectTransform _deactiavtedContent;

        protected List<ShopEyeItem> _shopEyeItems = new();
        protected IManager<ShopCustomizeManager, EyeCustomizeModel> _manager;

        public void SetManager(IManager<ShopCustomizeManager, EyeCustomizeModel> manager) => _manager = manager;
        private void Awake() => Init();
        protected virtual void InitData(int[] saveIndex)
        {
            
        }
        
        protected virtual void Init()
        {
            
        }

        public void SetData(GameData data)
        {
            this.WaitToObjectInitAndDo(_shopEyeItems[0], () =>
            {
                var index = transform.GetSiblingIndex();
                var lenght = data.EyeItemParameters[index].BaseEyeItems.Length;
                
                for (int i = 0; i < lenght; i++)
                {
                    _shopEyeItems[i].
                }
                
                foreach (var item in _shopEyeItems)
                {
                    
                }
            });
        }

        public GameData GetData()
        {
            /*List<BaseEyeItemParameters> baseEyeItemParameters = new();
            
            foreach (var item in _shopEyeItems)
            {
                baseEyeItemParameters.Add(item);
            }

            return new()
            {
                EyeItemParameters = baseEyeItemParameters.ToArray(),
            };*/

            return null;
        }
    }
}