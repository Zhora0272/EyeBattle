using Pooling;
using UnityEngine;

public abstract class GunAmmoBase : CachedMonoBehaviour, IPoolingMono
{
    internal abstract void Attack();
    protected abstract void Explosion();
    public MonoBehaviour PoolMonoObj { get; }
    
    public void PoolActivate()
    {
        gameObject.SetActive(true);
    }
    public void PoolDeactivate()
    {
        gameObject.SetActive(false);
    }

    public void PoolDestroy()
    {
        Destroy(gameObject);
    }

    public bool ActiveInHierarchy { get; }
}