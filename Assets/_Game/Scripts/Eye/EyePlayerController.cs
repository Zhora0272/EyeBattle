using DG.Tweening;
using System;
using UniRx;
using UnityEngine;

public class EyePlayerController : EyeBaseController
{
    [SerializeField] private InputController _inputController;
    [SerializeField] private Transform _eyeModelTransform;

    private IDisposable _updateDisposable;
    private IDisposable _pointerUpDisposable;

    private Vector3 _lastPosition;
    private bool _handlerState;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();

        _inputController.RegisterJoysticData(data => { _moveDirection = data; });


        _inputController.PointerDownStream.Subscribe(_ =>
        {
            //
            _eyeModelTransform.DOKill();
            _moveDirection = Vector2.zero;

            _handlerState = true;
            //

            _updateDisposable = Observable.EveryUpdate().Subscribe(_ =>
            {
                if ((transform.position - _lastPosition) != Vector3.zero)
                {
                    Quaternion rotation = Quaternion.LookRotation(transform.position - _lastPosition, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 4);
                }

                _eyeModelTransform.Rotate((Rb.velocity.magnitude * Time.deltaTime * Speed.Value * 4), 0, 0);
            }).AddTo(this);
        }).AddTo(this);


        _inputController.PointerUpStream.Subscribe(_ =>
        {
            _handlerState = false;

            _moveDirection = Vector2.zero;
            _pointerUpDisposable?.Dispose();
            _updateDisposable.Dispose();
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

    protected override void FixedUpdate()
    {
        _lastPosition = transform.position;
        base.FixedUpdate();
    }

    // random look position after any time idle standing 
}