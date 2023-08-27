using System;
using UniRx;
using UnityEngine;

public class EyePlayerController : MonoBehaviour
{
    [SerializeField] private BrokenEyePartsController _brokenEyePartsController;

    [SerializeField] private InputController _inputController;

    [SerializeField] private MeshRenderer _meshRenderer;

    [SerializeField] private SphereCollider _sphereCollider;

    [SerializeField] private Rigidbody _rb;

    [SerializeField] private Transform _directionObject;

    [SerializeField] private Material _material;

    private Vector2 _joystickDirection;
    private Vector3 _lastPosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _inputController.RegisterJoysticData(data =>
        {
            _joystickDirection = data;

        });
    }

    private void Start()
    {
        /*Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ => 
        {
            _brokenEyePartsController.Activate(_material);
            _sphereCollider.enabled = false;
            _meshRenderer.enabled = false;
            _rb.isKinematic = true;

        }).AddTo(this);*/
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

    private void DirectionCalculation()
    {
        var lastDirection = _lastPosition - transform.position;

        _directionObject.transform.position = transform.position + Vector3.up * 3;

        _directionObject.LookAt(lastDirection);

        _lastPosition = transform.position;
    }
}
