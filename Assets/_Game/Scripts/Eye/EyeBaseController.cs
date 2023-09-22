using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public interface IEyeParameters
{
    public IReactiveProperty<int> Hp { get; }
    public IReactiveProperty<float> Speed { get; }
    public IReactiveProperty<float> Force { get; }
    public Vector3 Position { get; }
}

public abstract class EyeBaseController : CachedMonoBehaviour, IEyeParameters
{
    public IReactiveProperty<int> Hp => _hp;
    public IReactiveProperty<float> Speed => _speed;
    public IReactiveProperty<float> Force => _force;
    public Vector3 Position => transform.position;

    //readonly reactive properties
    private readonly ReactiveProperty<int> _hp = new(100);
    private readonly ReactiveProperty<float> _speed = new(25);
    private readonly ReactiveProperty<float> _force = new();
    //

    [Space] [SerializeField] protected BrokenEyePartsController _brokenEyePartsController;
    [SerializeField] protected BrokenEyeCollection _brokenEyeCollector;
    [SerializeField] protected Rigidbody Rb;
    [Space] [SerializeField] protected Material _material;
    [SerializeField] protected GameObject _meshRenderer;
    [SerializeField] protected SphereCollider _sphereCollider;
    [Space] [SerializeField] protected Image _loadbar;

    [field: SerializeField] public ReactiveProperty<float> Size { protected set; get; }
    [field: SerializeField] public bool IsDeath { protected set; get; }

    protected Vector2 _moveDirection;

    #region UnityEvents
    protected virtual void Start()
    {
        _brokenEyeCollector.BrokenPartsCollectionStream.Subscribe(value => { Size.Value += value; }).AddTo(this);

        Size.Subscribe(value =>
        {
            _loadbar.DOFillAmount(((int) (value / 100) + 1) - ((float) value / 100), 1);

            if (value % 100 == 0)
            {
                transform.DOScale((value / 300) + 1, 1);

                _loadbar.DOKill();
                _loadbar.DOFillAmount(0, 1);
            }
        }).AddTo(this);
    }
    protected virtual void Update()
    {
        Rb.velocity = Vector3.Lerp(Rb.velocity, Vector3.zero, Time.deltaTime);
        Rb.angularVelocity = Vector3.Lerp(Rb.angularVelocity, Vector3.zero, Time.deltaTime);
    }
    protected virtual void FixedUpdate()
    {
        Move();
    }
    #endregion

    protected abstract void Move();

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Eye"))
        {
            if (collision.gameObject.TryGetComponent<EyeBaseController>(out var result))
            {
                result.Attack(Rb.mass * Rb.velocity.magnitude, transform.position);
            }
        }
    }

    protected virtual void Attack(float force, Vector3 attackPosition)
    {
        if (Hp.Value < force)
        {
            IsDeath = true;

            _brokenEyePartsController.Activate(_material, attackPosition);

            _sphereCollider.enabled = false;
            _meshRenderer.SetActive(false);
            Rb.isKinematic = true;
        }
    }

    
}