using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;


public class EyeBotController : EyeBaseController
{
    [SerializeField] private BotBattleParticipant _battleParticipant;

    private IEyeParameters _mineParameters;

    private IMoveableRigidbody _moveableRigidbody;

    private void Awake()
    {
        _mineParameters = this;
        _moveableRigidbody = new MoveWithRbAddTorque();
    }


    private void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(Random.Range(1f, 2f))).Subscribe(_ =>
        {
            if (_battleParticipant.GetClosestElement(out var result))
            {
                if (_mineParameters.Hp.Value >= result.Hp.Value)
                {
                    _moveDirection = result.Position - transform.position;
                }
            }
        }).AddTo(this);
    }

    protected override void Move()
    {
        _moveableRigidbody.Move(Rb, _moveDirection, Speed.Value);
    }
}