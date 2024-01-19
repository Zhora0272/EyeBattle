using Pooling;
using UniRx;
using UnityEngine;

public class UpdateElementController : MonoBehaviour, IPoolingMono
{
    [SerializeField] public UpdateElementModel UpdateElementModel;
    public MonoBehaviour PoolMonoObj => this;

    private void Awake()
    {
        UpdateElementModel.IsApplied.Subscribe(state =>
        {
            if (!state)
            {
                PoolActivate();
            }
            else
            {
                PoolDeactivate();
            }
        }).AddTo(this);
    }

    public void PoolActivate()
    {
        
    }

    public void PoolDeactivate()
    {
        gameObject.SetActive(false);
    }

    public void PoolDestroy()
    {
        
    }
}

public enum UpdateElement
{
    Speed,
}