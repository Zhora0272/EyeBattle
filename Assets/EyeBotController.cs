using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class EyeBotController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    private Vector2 _moveDirection;

    private void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(Random.Range(1f,2f))).Subscribe(_ =>
        {

        }).AddTo(this);
    }

    private void FixedUpdate()
    {
        if (_moveDirection != Vector2.zero)
        {
            _rb.AddTorque(
                new Vector3(_moveDirection.y, 0, -_moveDirection.x)
                * 50,
                ForceMode.Acceleration);
        }
    }
}
