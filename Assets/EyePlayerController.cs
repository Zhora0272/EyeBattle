using System;
using UniRx;
using UnityEngine;

public class EyePlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private InputController _inputController;

    [SerializeField] private Transform _directionObject;

    private Vector2 _joystickDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _inputController.RegisterJoysticData(data =>
        {
            _joystickDirection = data;

        });
    }

    private void Update()
    {
        DirectionCalculation();
    }

    private void FixedUpdate()
    {
        if (_joystickDirection != Vector2.zero)
        {
            _rb.AddTorque(
                new Vector3(_joystickDirection.y, 0, -_joystickDirection.x)
                * 50,
                ForceMode.Acceleration);
        }
    }


    private Vector3 _lastPosition;

    private void DirectionCalculation()
    {
        var lastDirection = _lastPosition - transform.position;

        _lastPosition = transform.position;

        _directionObject.transform.position = transform.position + Vector3.up * 3;

        _directionObject.LookAt(lastDirection);
    }
}
