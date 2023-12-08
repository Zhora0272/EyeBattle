using Random = UnityEngine.Random;
using UnityEngine;
using System;
using Pooling;
using UniRx;

public class EyeBotController : EyeBaseController, IPoolingMono
{
    [SerializeField] private BotBattleParticipant _battleParticipant;
    [Space] 
    [SerializeField] private BotBehaviourModel _model;

    private ReactiveProperty<BotState> _state = new(BotState.Idle);

    //
    public MonoBehaviour PoolMonoObj => this;

    //
    private IBotMonoBehaviour _botBehaviour;
    private IMoveableRigidbody _moveableRigidbody;
    private IEyeParameters _closestEyeElement;
    
    //
    private Vector3 _closestEnemyPosition;
    
    private Vector3 _currentMoveDirection;

    private void Awake()
    {
        _moveableRigidbody = new MoveWithRbAddForce();
        _botBehaviour = new BotMiddleBehaviour(_model);
    }

    private IDisposable _closestElementDisposable;
    private IDisposable _behaviourUpdateDisposable;
    
    private void Start()
    {
        _closestElementDisposable = Observable.Interval(
            TimeSpan.FromSeconds(Random.Range(0.5f, 1f))).Subscribe(_ =>
        {
            if (_battleParticipant.GetClosestElement(out var result))
            {
                _closestEyeElement = result;
                _closestEnemyPosition = result.EyeTransform.position;
            }
        }).AddTo(this);

        _behaviourUpdateDisposable = Observable.Interval(
                TimeSpan.FromSeconds(
                    Random.Range(0.5f, 1.5f)))
            .Subscribe(_ => { UpdateBehaviourState(); })
            .AddTo(this);

        _state.Subscribe(state =>
        {
            if (state == BotState.Idle)
            {
                MoveBalanceStop();
            }
            else
            {
                MoveBalanceStart();
            }
            
        }).AddTo(this);
    }

    private void UpdateBehaviourState()
    {
        _state.Value = _botBehaviour.BotBehaviourUpdate(this, _closestEyeElement);

        switch (_state.Value)
        {
            case BotState.RandomWalk:
                moveDirection = (HelperMath.GetRandomPosition(-1f, 1f) * 10).normalized;
                break;
            case BotState.Idle:
                moveDirection = Vector3.zero;
                break;
            case BotState.GoAwayFromEnemy:
                moveDirection = transform.position - _closestEnemyPosition;
                break;
            case BotState.Attack:
                moveDirection = _closestEnemyPosition - transform.position;
                break;
        }
    }


    protected override void Move()
    {
        _currentMoveDirection = Vector3.Lerp(
            _currentMoveDirection,
            moveDirection,
            Time.deltaTime);

        _moveableRigidbody.Move(Rb,
            _currentMoveDirection, 0.5f);
    }

    public void PoolActivate()
    {
        gameObject.SetActive(true);
    }

    public void PoolDeactivate()
    {
        _behaviourUpdateDisposable.Dispose();
        _closestElementDisposable.Dispose();
        gameObject.SetActive(false);
    }

    public void PoolDestroy()
    {
        Destroy(gameObject);
    }
}