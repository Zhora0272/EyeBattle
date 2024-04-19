using System;
using UniRx;
using UnityEngine;

public abstract class BaseBrokenEyeCollection : CachedMonoBehaviour
{
    [SerializeField] private TriggerCheckController _triggerCheckController;

    public IObservable<float> BrokenPartsCollectionStream => _brokenPartCollectionSubject;
    private Subject<float> _brokenPartCollectionSubject = new();

    private IDisposable _indicatorHideDisposable;

    protected virtual void Awake()
    {
        _triggerCheckController.TriggerLayerEnterRegister(Layer.BrokenEye,
            BrokenEyeEnterTrigger);
    }

    protected virtual void Start()
    {
        
    }

    private void OnEnable()
    {
        _triggerCheckController.EnableCollider();
    }

    private void OnDisable()
    {
        _triggerCheckController.DisableCollider();
    }

    private void BrokenEyeEnterTrigger(Collider other)
    {
        if (other.TryGetComponent<Collectable>(out var result))
        {
            if (result.CollectState) return;

            if (Time.time - result.BrokenTime < 1)
            {
                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
                        { Collect(result, other, 1); })
                    .AddTo(this);
            }
            else
            {
                Collect(result, other, 0.3f);
            }
        }
    }

    private void Collect(Collectable result, Collider other, float duration)
    {
        CollectAction(result.Collect(this, duration));
    }

    protected virtual void CollectAction(int value)
    {
        
    }
}