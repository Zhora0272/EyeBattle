using System;
using UnityEngine;
using UniRx;


public abstract class MoveableBattleParticipantBaseController : BattleParticipantBaseController
{
    public IReactiveProperty<float> Speed => _speed;

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
    [Space] [SerializeField] protected TriggerCheckController _triggerCheckController;

    [field: SerializeField] public ReactiveProperty<float> Size { protected set; get; }

    protected Vector3 moveDirection;
    private Vector3 _lastPosition;

    #region UnityEvents

    private IDisposable _everyUpdateDispose;

    public virtual void OnSpawned()
    {
        // Логика при активации из пула
        Debug.Log("Object Spawned");
    }

    public virtual void OnDespawned()
    {
        // Логика при деактивации (возврат в пул)
        Debug.Log("Object Despawned");
    }

    protected virtual void Awake()
    {
        Rb ??= GetComponent<Rigidbody>();

        _triggerCheckController.TriggerLayerEnterRegister(Layer.Xp, AttackCheck);
        _triggerCheckController.TriggerLayerEnterRegister(Layer.Coin, AttackCheck);
    }

    #endregion

    #region Attack Systeam

    private void AttackCheck(Collider other)
    {
        if (other.gameObject.TryGetComponent<MoveableBattleParticipantBaseController>(out var result))
        {
            //if (result.Force > Force)
            {
                //DeadEvent();
            }
        }
    }

    #endregion
}