using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class Collectable : CachedMonoBehaviour
{
    [field: SerializeField] private CollectableType _type;

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private MeshCollider _collider;
    [SerializeField] private float _value;

    private ITransform _target;

    public bool CollectState { private set; get; }
    public float BrokenTime { private set; get; }

    private CollectableCollectAnimBase _collectAnimBase;

    private void Awake()
    {
        _collectAnimBase = new CollectableCollectAnimation();
    }

    private void OnEnable()
    {
        BrokenTime = Time.time;
    }

    public float Collect(ITransform target, float duration)
    {
        if (CollectState) return 0;
        CollectState = true;
        
        Debug.Log(CollectState, gameObject);
        
        transform.parent = null;

        _collider.enabled = false;
        _rb.isKinematic = true;

        Observable.Timer(TimeSpan.FromSeconds(.5f))
            .Subscribe(_ =>
            {
                _collectAnimBase.Animation(this, target, duration);
            }).AddTo(this);
        return _value;
    }
}

public class CollectableCollectAnimation : CollectableCollectAnimBase
{
    public override void Animation(CachedMonoBehaviour mono, ITransform target, float duration)
    {
        mono.transform.DOMove(mono.IPosition + Vector3.up * 2, 2)  //jump up 
            .SetEase(Ease.OutBack)
            .onComplete = () =>
        {
            Observable.EveryUpdate().Subscribe(_ => //go to target
            {
                mono.Position = Vector3.Lerp(mono.Position,
                    target.IPosition + Vector3.up,
                    Time.deltaTime * 5);
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