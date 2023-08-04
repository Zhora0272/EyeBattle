using System;
using Cinemachine;
using UniRx;
using UnityEngine;

public class CameraAnimationController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _startCamera;
    [SerializeField] private CinemachineVirtualCamera _playCamera;

    private void Awake()
    {
        _startCamera.enabled = true;
        _playCamera.enabled = false;
        Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            _startCamera.enabled = false;
            _playCamera.enabled = true;
            
        }).AddTo(this);
    }
}