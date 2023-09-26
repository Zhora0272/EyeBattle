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
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime);
            _eyeModelTransform.Rotate((Rb.velocity.magnitude * Time.deltaTime * Speed.Value * 4), 0, 0);
        }
    }

    private void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(Random.Range(1f, 2f))).Subscribe(_ =>
        {
            if (_battleParticipant.GetClosestElement(out var result))
            {
                _target = result.EyeTransform.position;
                _closestEnemyPosition = result.EyeTransform.position;
            }
        }).AddTo(this);

        Observable.Interval(TimeSpan.FromSeconds(Random.Range(3f, 13f))).Subscribe(_ =>
        {
            _botActionState = (BotState)Random.Range(0, 3);
            
            /*switch (_botActionState)
            {
                case BotState.RandomWalk:
                {
                    _moveDirection = Helpers.GetRandomPosition(-1f, 1f);
                    break;
                }
                case BotState.Idle:
                {
                    _moveDirection = Vector3.zero;
                    break;
                }
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
            
            _moveDirection = Helpers.GetRandomPosition(-1f, 1f);

            _forceText.text = _botActionState.ToString();

        }).AddTo(this);
    }

    private bool _attackState;

    private Vector3 _randomDirection;
    private Vector3 _lastPosition;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_target, 1);
    }

    protected override void Move()
    {
        _lastPosition = transform.position;
        _moveableRigidbody.Move(Rb, _moveDirection, 0.5f);
    }


    private enum BotState
    {
        RandomWalk,
        Idle,
        Attack,
        GoAwayFromEnemies,
    }
}