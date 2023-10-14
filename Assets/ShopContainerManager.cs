using System.Collections.Generic;
using UnityEngine;

namespace Shop.Container
{
    public class ShopContainerManager : MonoBehaviour, ISaveable
    {
        private ShopContainerPart[] _containers;
        private void Awake()
        {
            _containers = GetComponentsInChildren<ShopContainerPart>();
        }

        private void Start()
        {
            foreach (ShopContainerPart container in _containers)
            {
                container.SetManager(this);
            }
        }

        private void OnEnable()
        {
            ActivateContainer(null);
        }

        internal void ActivateContainer(ShopContainerPart container)
        {
            foreach (ShopContainerPart part in _containers)
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
            for (int i = 0; i < _containers.Length; i++)
            {
                _containers[i].SetData(i);
            }
        }

        public GameData GetData()
        {
            List<int> indexes = new();
            
            foreach (var item in _containers)
            {
                indexes.Add(item.GetData());
            }
            
            return new GameData()
            {
                ContainerConfigIndexes = indexes.ToArray(),
            };
        }
    }
}
