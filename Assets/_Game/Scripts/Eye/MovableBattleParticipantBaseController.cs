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
    
    protected virtual void Update()
    {
       
    }

    internal virtual void EyeActivate()
    {
       
    }

    internal virtual void DeadEvent()
    {
        
    }

    private void BotStateChangeEvent(bool state)
    {
        
    }

    protected virtual void DisposeAll()
    {
       
    }

    #region Eye Balance in Idle mode

    protected void MoveBalanceStart()
    {
        
    }

    protected void MoveBalanceStop()
    {
    }

    #endregion
    

    #endregion

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