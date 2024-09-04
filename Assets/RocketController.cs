using UnityEngine;
using System;
using UniRx;

public class RocketController : GunAmmoBase
{
    [SerializeField] private GameObject _visualEffects;

    private Rigidbody _rb;
    private Vector3 _relativeForceVector;

    private float _speedCoeficient = 1;

    private void Awake()
    {
        _relativeForceVector = Vector3.forward * Time.fixedDeltaTime;
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _rb.isKinematic = true;
    }

    public override void PoolDeactivate()
    {
        base.PoolDeactivate();
        _fixedUpdate?.Dispose();

        _lookAtTargetUpdate?.Dispose();
        _everyUpdateDisposable?.Dispose();

        _visualEffects.SetActive(false);
    }

    private IDisposable _fixedUpdate;

    protected override void Explosion()
    {
        var result = Physics.OverlapSphere(IPosition, 15);

        foreach (var item in result)
        {
            if (item.TryGetComponent<Rigidbody>(out var rb))
            {
                print(rb.name);
                rb.AddExplosionForce(10000, (IPosition - item.transform.position) + Vector3.up, 15);
            }
        }

        PoolDeactivate();
    }

    internal override void Attack(ITransform target)
    {
        _fixedUpdate = Observable.EveryFixedUpdate().Subscribe(_ =>
        {
            _rb.AddRelativeForce(_relativeForceVector * _speedCoeficient, ForceMode.Impulse);
        }).AddTo(this);

        StartEngine();

        _speedCoeficient = 0;
        Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
        {
            _speedCoeficient = 1.5f;
            _rb.isKinematic = false;

            StartMoving();

            Observable.Timer(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
            {
                _checkController.TriggerLayerEnterRegister(new[] { Layer.Npc, Layer.Stronghold},
                    _ =>
                    { Explosion(); });

                GoToTarget(target);
                
            }).AddTo(this);
        }).AddTo(this);
    }

    private void StartEngine()
    {
        _visualEffects.SetActive(true);
    }

    private void StartMoving()
    {
        transform.SetParent(null);
    }

    private float _distance;

    private IDisposable _lookAtTargetUpdate;
    private IDisposable _everyUpdateDisposable;

    private void GoToTarget(ITransform target)
    {
        _speedCoeficient = 3;

        _everyUpdateDisposable = Observable.EveryUpdate().Subscribe(_ =>
        {
            _rb.velocity = Vector3.Lerp(_rb.velocity, Vector3.zero, Time.deltaTime);
            _rb.angularVelocity = Vector3.Lerp(_rb.angularVelocity, Vector3.zero, Time.deltaTime);
        }).AddTo(this);

        _lookAtTargetUpdate = Observable.EveryUpdate().Subscribe(_ =>
        {
            var direction = target.IPosition - Position;
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
}