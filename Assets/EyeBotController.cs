using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class EyeBotController : EyeBaseController
{
    private void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(Random.Range(1f,2f))).Subscribe(_ =>
        {
            _moveDirection = new Vector2(Random.Range(-1f,1), Random.Range(-1f, 1f));

        }).AddTo(this);
    }

    private void FixedUpdate()
    {
        if (_moveDirection != Vector2.zero)
        {
            Rb.AddTorque(
                new Vector3(_moveDirection.y, 0, -_moveDirection.x)
                * 50,
                ForceMode.Acceleration);
        }
    }
}
