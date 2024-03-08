using UnityEngine;
using System;
using UniRx;

public class RocketController : GunAmmoBase
{
    [SerializeField] private TriggerCheckController _checkController;
    [SerializeField] private Transform _target;

    private Rigidbody _rb;
    private Vector3 _relativeForceVector;

    private float _speedCoeficient = 1;

    private void Awake()
    {
        _relativeForceVector = Vector3.forward * Time.fixedDeltaTime;
        _rb = GetComponent<Rigidbody>();

        _checkController.TriggerLayerEnterRegister(Layer.Eye, _ => { Explosion(); });
        _checkController.TriggerLayerEnterRegister(Layer.Ground, _ => { Explosion(); });
    }

    protected override void Explosion()
    {
        PoolDeactivate();
    }

    internal override void Attack()
    {
        StartEngine();

        _speedCoeficient = 0;
        Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
        {
            _speedCoeficient = 1.5f;
            StartMoving();
            Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ => { GoToTarget(); }).AddTo(this);
        }).AddTo(this);
    }

    private void StartEngine()
    {
        
    }

    private void StartMoving()
    {
        
    }

    private float _distance;
    private IDisposable _lookAtTargetUpdate;

    private void GoToTarget()
    {
        _speedCoeficient = 3;

        _lookAtTargetUpdate = Observable.EveryUpdate().Subscribe(_ =>
        {
            var direction = _target.position - Position;
            _distance = direction.magnitude;

            if (_distance > 5)
            {
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    Quaternion.LookRotation(direction),
                    Time.deltaTime * _speedCoeficient * 5);
            }
        }).AddTo(this);
    }

    private void OnDisable()
    {
        _lookAtTargetUpdate?.Dispose();
    }

    private void Update()
    {
        _rb.velocity = Vector3.Lerp(_rb.velocity, Vector3.zero, Time.deltaTime);
        _rb.angularVelocity = Vector3.Lerp(_rb.angularVelocity, Vector3.zero, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        _rb.AddRelativeForce(_relativeForceVector * _speedCoeficient, ForceMode.Impulse);
    }
}