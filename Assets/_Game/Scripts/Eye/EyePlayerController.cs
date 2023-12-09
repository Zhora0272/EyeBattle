using DG.Tweening;
using System;
using UniRx;
using UnityEngine;

public class EyePlayerController : EyeBaseController
{
    [SerializeField] private InputController _inputController;

    private IDisposable _pointerUpDisposable;

    private IMoveableRigidbody _moveableRigidbody;
    
    private ReactiveProperty<bool> _handlerState = new();

    private IDisposable _rotateUpdateDisposable;
    private IDisposable _moveBalanceDisposable;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();

        _moveableRigidbody = new MoveWithRbAddForce(xyzState:false);
    }

    protected override void Start()
    {
        base.Start();

        _handlerState.Subscribe(state =>
        {
            if (state)
            {
                _moveBalanceDisposable?.Dispose();
                MoveBalanceStart();
            }
            else
            {
                _moveBalanceDisposable = Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
                {
                    MoveBalanceStop();
                    
                }).AddTo(this);
            }
            
        }).AddTo(this);

        //joystick update subscribe
        _inputController.RegisterJoysticData(data =>
        {
            moveDirection = data;
        });

        _inputController.PointerDownStream.Subscribe(_ =>
        {
            //
            _eyeModelTransform.DOKill();
            moveDirection = Vector2.zero;

            _handlerState.Value = true;
            //
            
        }).AddTo(this);


        _inputController.PointerUpStream.Subscribe(_ =>
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
                }
                
            }).AddTo(this);
            
        }).AddTo(this);
    }

    protected override void Move()
    {
        if (_handlerState.Value)
        {
            _moveableRigidbody.Move(Rb, moveDirection, 0.5f);
        }
    }
    // random look position after any time idle standing 
}