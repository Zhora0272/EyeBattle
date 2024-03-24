using Pooling;
using UnityEngine;

public abstract class GunAmmoBase : CachedMonoBehaviour, IPoolingMono
{
    internal abstract void Attack(ITransform target);
    protected abstract void Explosion();
    public MonoBehaviour PoolMonoObj { get; }
    
    public virtual void PoolActivate()
    {
        gameObject.SetActive(true);
    }
    public virtual void PoolDeactivate()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        PoolDeactivate();
    }

    public void PoolDestroy()
    {
        Destroy(gameObject);
    }

    public bool ActiveInHierarchy { get; }
}