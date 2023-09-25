using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;


public class EyeBotController : EyeBaseController
{
    [SerializeField] private BotBattleParticipant _battleParticipant;
    [SerializeField] private Transform _eyeModelTransform;

    private IEyeParameters _mineParameters;
    private IMoveableRigidbody _moveableRigidbody;

    private void Awake()
    {
        _mineParameters = this;
        _moveableRigidbody = new MoveWithRbAddForce();
    }

    private Vector3 _target;

    protected override void Update()
    {
        base.Update();

        if (_lastPosition != transform.position)
        {
            Quaternion rotation = Quaternion.LookRotation(transform.position - _lastPosition, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 4);
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
                _moveDirection = result.EyeTransform.position - transform.position;
            }
        }).AddTo(this);

        Observable.Interval(TimeSpan.FromSeconds(Random.Range(3f, 13f))).Subscribe(_ =>
        {
            _attackState = !_attackState;

            _randomDirection = Helpers.GetRandomPosition(-1f, 1f);
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
        _moveableRigidbody.Move(Rb, _attackState ? _moveDirection : _randomDirection, 0.3f);
    }
}