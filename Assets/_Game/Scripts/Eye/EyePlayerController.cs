using DG.Tweening;
using System;
using UniRx;
using UnityEngine;

public class EyePlayerController : EyeBaseController
{
    [SerializeField] private InputController _inputController;

    private IDisposable _pointerUpDisposable;
    private bool _handlerState;

    private IMoveableRigidbody _moveableRigidbody;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();

        _moveableRigidbody = new MoveWithRbAddForce(xyzState:false);
    }

    protected override void Start()
    {
        base.Start();

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

            _handlerState = true;
            //
            
        }).AddTo(this);


        _inputController.PointerUpStream.Subscribe(_ =>
        {
            _handlerState = false;

            moveDirection = Vector2.zero;
            _pointerUpDisposable?.Dispose();
            _eyeModelTransform.DOKill();

            _pointerUpDisposable = Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
            {
                if (!_handlerState)
                {
                    _eyeModelTransform.DORotate(new Vector3(65, 180, 0), 1);
                    transform.DORotate(new Vector3(0, 180, 0), 1);
                }
                
            }).AddTo(this);
            
        }).AddTo(this);
    }

    protected override void Move()
    {
        _moveableRigidbody.Move(Rb, moveDirection, 0.5f);
    }

    // random look position after any time idle standing 
}