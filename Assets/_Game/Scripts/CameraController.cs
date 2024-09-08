using System;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : CachedMonoBehaviour
{
    [SerializeField] private InputController _inputController;
    [Space]
    [SerializeField] private Transform _target;
    [Space]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _scrollSpeed = 1.5f;
    [SerializeField] private float _freeSpaceSpeed = 1.5f;
    [Space] 
    [SerializeField] private float _distanceMax = 4f;
    [SerializeField] private float _distanceMin = 1.5f;
    [Space] 
    [SerializeField] private Vector3 _offset;
    
    private readonly ReactiveProperty<bool> _cameraTargetModeProperty = new(true);

    private IDisposable _lateUpdateDisposable;

    private float _distance;
    

    private void OnEnable()
    {
        _distance = _distanceMax;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Position + transform.forward, 1);
    }

    private void Awake()
    {
        _cameraTargetModeProperty.Subscribe(targetMode =>
        {
            _lateUpdateDisposable?.Dispose();

            if (targetMode)
            {
                _lateUpdateDisposable = Observable.EveryLateUpdate().Subscribe(_ =>
                {
                    CameraPositionUpdate(_target.position);
                    
                }).AddTo(this);
            }
            else
            {
                var targetPosition = _target.position;
                
                _lateUpdateDisposable = Observable.EveryLateUpdate().Subscribe(_ =>
                {
                    var newPos = _inputController.ScreenEdgeInputProperty.Value * _freeSpaceSpeed * _distance;
                    
                    targetPosition = new Vector3(targetPosition.x + newPos.x, 0, targetPosition.z + newPos.y);
                    
                    CameraPositionUpdate(targetPosition);

                }).AddTo(this);
            }
        }).AddTo(this);
    }

    private void CameraPositionUpdate(Vector3 targetPosition)
    {
        Position = Vector3.Lerp(
            Position,
            targetPosition + _offset * (_distance * 10),
            Time.deltaTime * _moveSpeed);
    }

    private void Start()
    {
        _inputController.SpacePressProperty.Skip(1).Subscribe(_ =>
        {
            CameraModeChange();
        }).AddTo(this);

        _inputController.MouseScrollProperty.Subscribe(scrollVector =>
        {
            var scrollValue = _distance + scrollVector * _scrollSpeed * Time.deltaTime;
            _distance = Mathf.Clamp(scrollValue, _distanceMin, _distanceMax);
            
        }).AddTo(this);
    }

    private void SetCameraTargetMode()
    {
        _cameraTargetModeProperty.Value = true;
    }

    private void SetCameraFreeSpaceMode()
    {
        _cameraTargetModeProperty.Value = false;
    }
    
    private void CameraModeChange()
    {
        _cameraTargetModeProperty.Value = !_cameraTargetModeProperty.Value;
    }
}