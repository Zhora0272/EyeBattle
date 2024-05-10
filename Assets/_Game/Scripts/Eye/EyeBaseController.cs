using System;
using Lofelt.NiceVibrations;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public abstract class EyeBaseController : CachedMonoBehaviour,
    IEyeParameters, IEyebattleParameters, IUpdateable<UpdateElementModel>
{
    [SerializeField] private int clanId;
    //
    [SerializeField] private Transform _rotationTrasform;
    //
    
    public IReactiveProperty<int> Mass => _hp;
    public IReactiveProperty<float> Speed => _speed;
    public float Force => Rb.mass * Rb.velocity.magnitude;
    public ITransform EyeTransform => this;
    public IReactiveProperty<int> KillCount => _killCount;
    public int ClanId => clanId;
    public IReactiveProperty<bool> IsDeath => _isDeath;
    //


    //readonly reactive properties
    private readonly ReactiveProperty<int> _hp = new(100);
    private readonly ReactiveProperty<int> _killCount = new(0);
    private readonly ReactiveProperty<bool> _isDeath = new();

    protected readonly ReactiveProperty<float> _speed = new(25);
    //

    [Space]
    [SerializeField] protected BrokenEyePartsController _brokenEyePartsController;
    [SerializeField] protected BaseBrokenEyeCollection _brokenEyeCollector;
    [SerializeField] protected Rigidbody Rb;
    [Space] 
    [SerializeField] protected GameObject _meshRenderer;
    [SerializeField] protected Transform _eyeModelTransform;
    [SerializeField] protected SphereCollider _sphereCollider;
    [Space]
    [SerializeField] protected Image _loadbar;
    [SerializeField] protected TriggerCheckController _triggerCheckController;
    [Space]
    [SerializeField] private UpdateController _updateController;

    [field: SerializeField] public ReactiveProperty<float> Size { protected set; get; }

    protected Vector3 moveDirection;
    private Vector3 _lastPosition;

    #region UnityEvents

    private IDisposable _everyUpdateDispose;

    protected virtual void Awake()
    {
        Rb ??= GetComponent<Rigidbody>();

        _currentModelClone = GetEyeConfigModel();
        _triggerCheckController.TriggerLayerEnterRegister(Layer.Eye, EyeAttackCheck);
        //_updateController.UpdateElementController.Subscribe(GetUpdate).AddTo(this);
    }

    private void FixedUpdate()
    {
        FixedMove();
    }

    protected virtual void Update()
    {
        if ((Position - _lastPosition).magnitude > .1f)
        {
            EyeRotate();
        }

        if (Time.frameCount % 10 == 0)
        {
            _lastPosition = Position;
        }
    }

    #region Updateable part

    private EyeModelBase _currentModelClone;
    private IDisposable _cancelUpdateDisposable;

    public void GetUpdate(UpdateElementModel model)
    {
        if (model == null) return;

        _currentModelClone = GetEyeConfigModel();

        AddUpdate(model);

        if (model.UpdateTime > 0)
        {
            _cancelUpdateDisposable = Observable.Timer(TimeSpan.FromSeconds(model.UpdateTime)).Subscribe(_ =>
            {
                CancelUpdate();
            }).AddTo(this);
        }
        else
        {
            SetupNewModel(model);
        }
    }

    private void AddUpdate(UpdateElementModel model)
    {
        _speed.Value += model.Speed;
    }

    private void CancelUpdate()
    {
        _speed.Value = _currentModelClone.Speed;
    }

    private void SetupNewModel(UpdateElementModel model)
    {
        _currentModelClone.Speed = model.Speed;
    }

    private EyeModelBase GetEyeConfigModel()
    {
        return new EyeModelBase()
        {
            Speed = _speed.Value,
        };
    }

    #endregion

    internal virtual void EyeActivate()
    {
        EyeStateChangeEvent(false);
    }
    
    internal virtual void EyeDeadEvent() 
    {
        DisposeAll();
        EyeStateChangeEvent(true);
        _brokenEyePartsController.Activate();
    }

    private void EyeStateChangeEvent(bool state)
    {
        Rb.isKinematic = state;
        _brokenEyeCollector.enabled = !state;
        _isDeath.Value = state;
        _sphereCollider.enabled = !state;
        
        if (!state)
        {
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                _meshRenderer.SetActive(!state);
            }).AddTo(this);
        }
        else
        {
            _meshRenderer.SetActive(!state);
        }
    }

    protected virtual void DisposeAll()
    {
        _cancelUpdateDisposable?.Dispose();
        _everyUpdateDispose?.Dispose();
    }
    
    #region Eye Balance in Idle mode

    protected void MoveBalanceStart()
    {
        MoveBalanceStop();
        _everyUpdateDispose = Observable.EveryFixedUpdate().Subscribe(_ =>
        {
            Rb.velocity = Vector3.Lerp(Rb.velocity, Vector3.zero, Time.fixedDeltaTime);
            Rb.angularVelocity = Vector3.Lerp(Rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime);
            
        }).AddTo(this);
    }

    protected void MoveBalanceStop()
    {
        _everyUpdateDispose?.Dispose();
    }

    #endregion

    private void EyeRotate()
    {
        var vectorOfPositions = transform.position - _lastPosition;
        
        Quaternion rotation = Quaternion.LookRotation(vectorOfPositions, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10);
        _rotationTrasform.Rotate((Rb.velocity.magnitude * Time.deltaTime * 80), 0, 0);
    }

    #endregion

    protected abstract void FixedMove();

    #region Attack Systeam

    private void EyeAttackCheck(Collider other)
    {
        if (other.gameObject.TryGetComponent<EyeBaseController>(out var result))
        {
            if (result.Force > Force)
            {
                EyeDeadEvent();
            }
        }
    }

    #endregion
}