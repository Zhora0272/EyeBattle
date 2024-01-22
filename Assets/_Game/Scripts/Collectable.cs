using DG.Tweening;
using UniRx;
using UnityEngine;

public class Collectable : CachedMonoBehaviour
{
    [field: SerializeField] private CollectableType _type;

    [SerializeField] private Rigidbody _rb;
    [SerializeField] private MeshCollider _collider;
    [SerializeField] private float _value;
    
    private ITransform target;

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
        transform.parent = null;
    }

    public float Collect(ITransform target, float duration)
    {
        CollectState = true;

        _collider.enabled = false;
        _rb.isKinematic = true;
        
        /*transform.DOScale(0, 0.5f).onComplete = () =>
        {
            gameObject.SetActive(false);
        };*/
        
        _collectAnimBase.Animation(this, target, duration);
        this.target = target;
        return _value;
    }
    
    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(target.IPosition, 1);
        }
    }
}

public class CollectableCollectAnimation : CollectableCollectAnimBase
{
    public override void Animation(CachedMonoBehaviour mono, ITransform target, float duration)
    {
        /*mono.transform.DOMove(mono.transform.position + Vector3.up * 3, duration).onComplete = () =>
        {
        };*/
        
        Observable.EveryUpdate().Subscribe(_ =>
        {
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
