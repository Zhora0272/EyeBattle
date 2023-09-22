using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class LookAtWorldPosition : MonoBehaviour
{
    [SerializeField] private Vector3 _lookRotation;

    private IDisposable _rotationUpdate;
    private void OnEnable()
    {
        _rotationUpdate = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            transform.DORotate(_lookRotation, 1);
        });
    }

    private void OnDisable()
    {
        _rotationUpdate.Dispose();
    }
}
