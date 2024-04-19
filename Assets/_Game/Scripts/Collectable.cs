using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class Collectable : CachedMonoBehaviour
{
    [field: SerializeField] private CollectableType _type;

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private MeshCollider _collider;
    [SerializeField] private int _value;

    private ITransform _target;

    public bool CollectState { private set; get; }
    public float BrokenTime { private set; get; }

    private CollectableCollectAnimBase _collectAnimBase;

    /*private void OnValidate()
    {
        _collider = GetComponent<MeshCollider>();
    }*/

    private void Awake()
    {
        _collectAnimBase = new CollectableCollectAnimation();
    }

    private void OnEnable()
    {
        BrokenTime = Time.time;
    }

    private void OnDisable()
    {
        _collectAnimBase.Deactivate(this);
    }

    public int Collect(ITransform target, float duration)
    {
        if (CollectState) return 0;
        CollectState = true;
        
        _collider.enabled = false;
        _rb.isKinematic = true;

        Observable.Timer(TimeSpan.FromSeconds(.5f))
            .Subscribe(_ =>
            {
                _collectAnimBase.Activate(this, target, duration);
            }).AddTo(this);
        return _value;
    }
}

public class CollectableCollectAnimation : CollectableCollectAnimBase
{
    private IDisposable _everyUpdate;
    
    public override void Deactivate(CachedMonoBehaviour mono)
    {
        mono.DOKill();
        _everyUpdate?.Dispose();
    }

    public override void Activate(CachedMonoBehaviour mono, ITransform target, float duration)
    {
        mono.transform.DOMove(mono.IPosition + Vector3.up * 2, 2)  //jump up 
            .SetEase(Ease.OutBack)
            .onComplete = () =>
        {
            _everyUpdate = Observable.EveryUpdate().Subscribe(_ => //go to target
            {
                mono.Position = Vector3.Lerp(mono.Position,
                    target.IPosition + Vector3.up,
                    Time.deltaTime * 10);
            }).AddTo(mono);
            
            mono.transform.DOScale(0, 2).onComplete = () => //move scale to the zero 
            {
                mono.gameObject.SetActive(false);
            };
        };
    }
}

public enum CollectableType
{
    BrokenEye,
    Updatable
}