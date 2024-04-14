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
            _conShopViewBases = GetComponentsInChildren<ShopViewBase>();
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
            var lenght = data.EyeItemParameters.Length;
            for (int i = 0; i < lenght; i++)
            {
                _conShopViewBases[i].SetData(
                    (data.ContainerConfigIndexes[i],
                        data.EyeItemParameters[i]));
            }
        }

        public GameData GetData()
        {
            List<EyeItemCollection> eyeItemCollections = new();
            List<int> containerIndex = new();

            foreach (var item in _conShopViewBases)
            {
                var data = item.GetData();
                eyeItemCollections.Add(data.Item2);
                containerIndex.Add(item.SelectedIndex.Value);
            }

            return new GameData()
            {
                EyeItemParameters = eyeItemCollections.ToArray(),
                ContainerConfigIndexes = containerIndex.ToArray(),
            };
        }
    }
}