using System;
using UnityEngine;
using UniRx;

public abstract class MovableBattleParticipantBaseController : BattleParticipantBaseController, IBotBattleParameters
{
    public IReactiveProperty<float> Speed => _speed;
    public IReactiveProperty<int> KillCount => _killCount;
    public IReactiveProperty<bool> IsDeath => _isDeath;
    //


    //readonly reactive properties
    private readonly ReactiveProperty<int> _hp = new(100);
    private readonly ReactiveProperty<int> _killCount = new(0);
    private readonly ReactiveProperty<bool> _isDeath = new();

    protected readonly ReactiveProperty<float> _speed = new(25);
    //

    [Space] [SerializeField] protected BaseCollectionController collector;
    [SerializeField] protected Rigidbody Rb;
    [Space] [SerializeField] protected Transform _botModelTransform;
    [SerializeField] protected Collider _sphereCollider;
    [Space] [SerializeField] protected TriggerCheckController _triggerCheckController;

    [field: SerializeField] public ReactiveProperty<float> Size { protected set; get; }

    protected Vector3 moveDirection;
    private Vector3 _lastPosition;

    #region UnityEvents

    private IDisposable _everyUpdateDispose;

    protected virtual void Awake()
    {
        Rb ??= GetComponent<Rigidbody>();

        _triggerCheckController.TriggerLayerEnterRegister(Layer.Xp, AttackCheck);
        _triggerCheckController.TriggerLayerEnterRegister(Layer.Coin, AttackCheck);
    }

    private void FixedUpdate()
    {
        FixedMove();
    }

    //Beta / need update 
    protected virtual void Update()
    {
        if ((Position - _lastPosition).magnitude > .1f)
        {
            Rotate();
        }

        if (Time.frameCount % 10 == 0)
        {
            _lastPosition = Position;
        }
    }

    internal virtual void EyeActivate()
    {
        BotStateChangeEvent(false);
    }

    internal virtual void DeadEvent()
    {
        DisposeAll();
        BotStateChangeEvent(true);
    }

    private void BotStateChangeEvent(bool state)
    {
        Rb.isKinematic = state;
        _isDeath.Value = state;
        _sphereCollider.enabled = !state;
    }

    protected virtual void DisposeAll()
    {
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

    private void Rotate()
    {
        var vectorOfPositions = transform.position - _lastPosition;

        Quaternion rotation = Quaternion.LookRotation(vectorOfPositions, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 10);
    }

    #endregion

    protected abstract void FixedMove();

    #region Attack Systeam

    private void AttackCheck(Collider other)
    {
        if (other.gameObject.TryGetComponent<MovableBattleParticipantBaseController>(out var result))
        {
            //if (result.Force > Force)
            {
                DeadEvent();
            }
        }
    }

    #endregion
}