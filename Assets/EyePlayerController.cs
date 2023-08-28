using UnityEngine;

public class EyePlayerController : EyeBaseController
{
    [SerializeField] private InputController _inputController;
   
    private Vector3 _lastPosition;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();

        _inputController.RegisterJoysticData(data =>
        {
            _moveDirection = data;

        });
    }

    private void FixedUpdate()
    {
        if (_moveDirection != Vector2.zero)
        {
            Rb.AddTorque(
                new Vector3(_moveDirection.y, 0, -_moveDirection.x)
                * Speed,
                ForceMode.Acceleration);
        }
    }

    private void DirectionCalculation()
    {
        var lastDirection = _lastPosition - transform.position;

        _lastPosition = transform.position;
    }
}
