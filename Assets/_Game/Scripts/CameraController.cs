using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _distance;
    [SerializeField] private float _smooth;

    [Header("Configs")] 
    [SerializeField] private float _inGameDistance = 4f;
    [SerializeField] private float _outGameDistance = 1.5f;

    private UIManager uiManager;

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            _target.position + ((_offset * 10) * _distance),
            Time.deltaTime * _smooth);
    }

    private void Awake()
    {
        uiManager = MainManager.GetManager<UIManager>();

        uiManager.SubscribeToPageActivate(UIPageType.InGame, () =>
        {
            _distance = _inGameDistance;
        });
        uiManager.SubscribeToPageActivate(UIPageType.TapToPlay, () =>
        {
            _distance = _outGameDistance;
        });
    }

    private void Start()
    {
       
    }
}