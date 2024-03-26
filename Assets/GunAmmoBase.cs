using Pooling;
using UnityEngine;

public abstract class GunAmmoBase : CachedMonoBehaviour, IPoolingMono
{
    [SerializeField] protected TriggerCheckController _checkController;

    internal abstract void Attack(ITransform target);
    protected abstract void Explosion();
    public MonoBehaviour PoolMonoObj { get; }
    
    public virtual void PoolActivate()
    {
        gameObject.SetActive(true);
    }
    public virtual void PoolDeactivate()
    {
        _checkController.TriggerLayerExitRegister(Layer.Eye, _ => { Explosion(); });
        _checkController.TriggerLayerExitRegister(Layer.Ground, _ => { Explosion(); });
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