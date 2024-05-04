using DG.Tweening;
using System;
using UniRx;
using UnityEngine;

public class EyePlayerController : EyeBaseController
{
    [SerializeField] private InputController _inputController;

    private IMoveableRigidbody _moveableRigidbody;
    private ReactiveProperty<bool> _handlerState = new();

    private IDisposable _pointerUpDisposable;
    private IDisposable _rotateUpdateDisposable;
    private IDisposable _moveBalanceDisposable;
    private IDisposable _pointerUpStreamDisposable;
    private IDisposable _pointerDownStreamDisposable;

    public IReactiveProperty<bool> DeadState => _deadState;
    public ReactiveProperty<bool> _deadState = new(false);

    protected override void Awake()
    {
        base.Awake();
        _moveableRigidbody = new MoveWithRbAddForce(xyzState: false);
    }

    internal override void EyeDeadEvent()
    {
        base.EyeDeadEvent();

        _handlerState?.Dispose();
        _moveBalanceDisposable?.Dispose();
        _pointerUpStreamDisposable?.Dispose();
        _pointerDownStreamDisposable?.Dispose();
        _pointerUpDisposable?.Dispose();
        _deadState.Value = true;
    }

    internal override void EyeActivate()
    {
        base.EyeActivate();
        ReadyToPlay();
        _brokenEyePartsController.ReActivate();
    }

    private void Start()
    {
        _inputController.RegisterJoysticData(data => { moveDirection = data; });
    }

    private void ReadyToPlay()
    {
        _deadState.Value = false;
        _handlerState.Subscribe(state =>
        {
            if (state)
            {
                _moveBalanceDisposable?.Dispose();
                MoveBalanceStart();
            }
            else
            {
                _moveBalanceDisposable = Observable.Timer(TimeSpan.FromSeconds(1))
                    .Subscribe(_ => { MoveBalanceStop(); }).AddTo(this);
            }
        }).AddTo(this);

        //joystick update subscribe

        _pointerDownStreamDisposable = _inputController.PointerDownStream.Subscribe(_ =>
        {
            //
            _eyeModelTransform.DOKill();
            moveDirection = Vector2.zero;
 
            _handlerState.Value = true;
            //
        }).AddTo(this);


        _pointerUpStreamDisposable = _inputController.PointerUpStream.Subscribe(_ =>
        {
            _handlerState.Value = false;

            moveDirection = Vector2.zero;

            _pointerUpDisposable?.Dispose();

            _pointerUpDisposable = Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
            {
                if (!_handlerState.Value)
                {
                    _eyeModelTransform.DORotate(new Vector3(65, 180, 0), 1);
                    transform.DORotate(new Vector3(0, 180, 0), 1);

                    Observable.Timer(TimeSpan.FromSeconds(.5f)).Subscribe(_ =>
                    {
                        Rb.velocity = Vector3.zero;
                        Rb.angularVelocity = Vector3.zero;
                    }).AddTo(this);
                }
            }).AddTo(this);
        }).AddTo(this);
    }

    protected override void Move()
    {
        if (_handlerState.Value)
        {
            _moveableRigidbody.Move(Rb, moveDirection, Speed.Value / 50);
        }
    }
    // random look position after any time idle standing 
}