using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class EyeBotController : EyeBaseController
{
    [SerializeField] private BotBattleParticipant _battleParticipant;
    [SerializeField] private Transform _eyeModelTransform;
    [Space] [SerializeField] private BotBehaviourModel _model;

    private IBotMonoBehaviour _botBehaviour;
    private IMoveableRigidbody _moveableRigidbody;

    private Vector3 _closestEnemyPosition;
    private IEyeParameters _closestEyeElement;

    private Vector3 _lastPosition;
    private Vector3 _currentMoveDirection;

    private void Awake()
    {
        _moveableRigidbody = new MoveWithRbAddForce();
        _botBehaviour = new BotMiddleBehaviour(_model);
    }

    protected override void Update()
    {
        base.Update();

        if (_lastPosition != transform.position)
        {
            Quaternion rotation = Quaternion.LookRotation(transform.position - _lastPosition, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5);
            _eyeModelTransform.Rotate((Rb.velocity.magnitude * Time.deltaTime * Speed.Value * 4), 0, 0);
        }
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
                _moveDirection = (Helpers.GetRandomPosition(-1f, 1f) * 10).normalized;
                break;
            case BotState.Idle:
                _moveDirection = Vector3.zero;
                break;
            case BotState.GoAwayFromEnemy:
                _moveDirection = transform.position - _closestEnemyPosition;
                break;
            case BotState.Attack:
                _moveDirection = _closestEnemyPosition - transform.position;
                break;
        }
    }

    protected override void Move()
    {
        _lastPosition = transform.position;

        _currentMoveDirection = Vector3.Lerp(
            _currentMoveDirection,
            _moveDirection,
            Time.deltaTime);

        _moveableRigidbody.Move(Rb,
            _currentMoveDirection, 0.5f);
    }
}