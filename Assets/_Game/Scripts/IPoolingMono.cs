using UnityEngine;

namespace Pooling
{
    public interface IPoolingMono
    {
        public MonoBehaviour PoolMonoObj { get; }
        public void PoolActivate();
        public void PoolDeactivate();
        public void PoolDestroy();
        public bool ActiveInHierarchy { get; }
    }
}