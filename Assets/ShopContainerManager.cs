using UnityEngine;

namespace Shop.Container
{
    public class ShopContainerManager : MonoBehaviour
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
    }
}
