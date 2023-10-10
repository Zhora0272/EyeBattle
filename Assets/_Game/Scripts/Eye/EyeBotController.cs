using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class EyeBotController : EyeBaseController
{
    [SerializeField] private BotBattleParticipant _battleParticipant;
    [Space] [SerializeField] private BotBehaviourModel _model;

    private IBotMonoBehaviour _botBehaviour;
    private IMoveableRigidbody _moveableRigidbody;

    private Vector3 _closestEnemyPosition;
    private IEyeParameters _closestEyeElement;

    private Vector3 _currentMoveDirection;

    private void Awake()
    {
        _moveableRigidbody = new MoveWithRbAddForce();
        _botBehaviour = new BotMiddleBehaviour(_model);
    }

    private void OnDrawGizmos()
    {
        if(_closestEyeElement == null) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_closestEyeElement.Position, 0.5f);
    }

    private void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(Random.Range(0.5f, 1f))).Subscribe(_ =>
        {
            if (_battleParticipant.GetClosestElement(out var result))
            {
                _closestEyeElement = result;
                _closestEnemyPosition = result.EyeTransform.position;
            }
        }).AddTo(this);

        Observable.Interval(
                TimeSpan.FromSeconds(
                    Random.Range(0.5f, 1.5f)))
            .Subscribe(_ => { UpdateBehaviourState(); })
            .AddTo(this);
    }

    private void UpdateBehaviourState()
    {
        var state = _botBehaviour.BotBehaviourUpdate(this, _closestEyeElement);

        switch (state)
        {
            case BotState.RandomWalk:
                moveDirection = (Helper.GetRandomPosition(-1f, 1f) * 10).normalized;
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
}