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
    public float BrokenTime{ private set; get; }

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
        transform.parent = null;
        
        _collider.enabled = false;
        _rb.isKinematic = true;

        Observable.Timer(TimeSpan.FromSeconds(3)).Subscribe(_ =>
        {
            transform.DOScale(0, 2).onComplete = () =>
            {
                gameObject.SetActive(false);
            };
            _collectAnimBase.Animation(this, target, duration);
            
        }).AddTo(this);
        return _value;
    }
}

public class CollectableCollectAnimation : CollectableCollectAnimBase
{
    public override void Animation(CachedMonoBehaviour mono, ITransform target, float duration)
    {
        Debug.Log(target.IGameObjectName);
        Debug.Log(mono.IGameObjectName);
        
        Observable.EveryUpdate().Subscribe(_ =>
        {
            Debug.DrawLine(mono.Position, target.IPosition, Color.red);
            mono.Position = Vector3.Lerp(mono.Position,
                target.IPosition + Vector3.up,
                Time.deltaTime);

        }).AddTo(mono);
    }
}

public enum CollectableType
{
    BrokenEye,   
    Updatable
}
