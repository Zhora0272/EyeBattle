using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;


public class EyeBotController : EyeBaseController
{
    [SerializeField] private BotBattleParticipant _battleParticipant;
    [SerializeField] private Transform _eyeModelTransform;

    private IEyeParameters _mineParameters;
    private IMoveableRigidbody _moveableRigidbody;

    private BotState _botActionState;

    private Vector3 _closestEnemyPosition;
    private Vector3 _target;

    private void Awake()
    {
        _mineParameters = this;
        _moveableRigidbody = new MoveWithRbAddForce();
    }

    protected override void Update()
    {
        base.Update();

        if (_lastPosition != transform.position)
        {
            Quaternion rotation = Quaternion.LookRotation(transform.position - _lastPosition, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 3);
            _eyeModelTransform.Rotate((Rb.velocity.magnitude * Time.deltaTime * Speed.Value * 4), 0, 0);
        }
    }

    private void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(Random.Range(0.5f, 1f))).Subscribe(_ =>
        {
            if (_battleParticipant.GetClosestElement(out var result))
            {
                _target = result.EyeTransform.position;
                _closestEnemyPosition = result.EyeTransform.position;
            }
        }).AddTo(this);

        Observable.Interval(TimeSpan.FromSeconds(Random.Range(0.5f, 1.5f))).Subscribe(_ => { SetRandomState(); })
            .AddTo(this);
    }

    private void SetRandomState()
    {
        //_botActionState = (BotState) Random.Range(0, 3);

        /*switch (_botActionState)
        {
            case BotState.RandomWalk:
            {
                _moveDirection = Helpers.GetRandomPosition(-1f, 1f);
                break;
            }
            /*case BotState.Idle:
            {
                _moveDirection = Vector3.zero;
                break;
            }#1#
            case BotState.GoAwayFromEnemies:
            {
                _moveDirection = transform.position - _closestEnemyPosition;
                break;
            }
            case BotState.Attack:
            {
                _moveDirection = _closestEnemyPosition - transform.position;
                break;
            }
        }*/

        //_moveDirection = Helpers.GetRandomPosition(-1f, 1f) * 10;

        _moveDirection = _closestEnemyPosition - transform.position;

        //_forceText.text = _botActionState.ToString();
    }

    private bool _attackState;

    private Vector3 _randomDirection;
    private Vector3 _lastPosition;

    private Vector3 _currentMoveDirection;

    private void OnDrawGizmos()
    {
        if (!IsDeath.Value)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(_moveDirection, 1);

            var direction = Vector3.zero;

            for (int i = 0; i < 10; i++)
            {
                Gizmos.DrawSphere(_closestEnemyPosition + (transform.position - _closestEnemyPosition) * (i * 0.100f),
                    0.3f);
            }
        }
    }

    protected override void Move()
    {
        _currentMoveDirection = Vector3.Lerp(_currentMoveDirection, _moveDirection, Time.deltaTime);

        _lastPosition = transform.position;
        _moveableRigidbody.Move(Rb, _currentMoveDirection, 0.5f);
    }


    private enum BotState
    {
        RandomWalk,

        //Idle,
        Attack,
        GoAwayFromEnemies,
    }
}