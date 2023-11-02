using System.Collections.Generic;
using Saveing;
using UnityEngine;

namespace Shop.Container
{
    public class ShopContainerManager : MonoBehaviour, IGameDataSaveable
    {
        [SerializeField] private ShopContainer[] _containers;
        [SerializeField] private ShopViewBase[] _conShopViewBases;

#if UNITY_EDITOR
        [ContextMenu("Set Children")]
        private void SetChildren()
        {
            _containers = GetComponentsInChildren<ShopContainer>();
            _conShopViewBases ??= GetComponentsInChildren<ShopViewBase>();
        }
#endif

        private void Start()
        {
            foreach (ShopContainer container in _containers)
            {
                container.SetManager(this);
            }

            ActivateContainer(null);
        }

        internal void ActivateContainer(ShopContainer container)
        {
            foreach (ShopContainer part in _containers)
            {
                if (part.IsActivated.Value)
                {
                    part.Deactivate();
                }
            }

            if (container && !container.IsActivated.Value)
            {
                container.Activate();
            }
        }

        public void SetData(GameData data)
        {
            for (int i = 0; i < data.EyeItemParameters.Length; i++)
            {
                _conShopViewBases[i].SetData(data.EyeItemParameters[i]);
            }
        }

        public GameData GetData()
        {
            List<int> indexes = new();

            foreach (var item in _containers)
            {
                //indexes.Add(item.GetData());
            }

            return new GameData()
            {
                ContainerConfigIndexes = indexes.ToArray(),
            };
        }
    }
}