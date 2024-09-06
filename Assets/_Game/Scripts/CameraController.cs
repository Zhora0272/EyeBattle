using UniRx;
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
    
    [SerializeField] private InputController _inputController;

    private UIManager uiManager;

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

    private bool _cameraMode;
    private void Start()
    {
        _inputController.SpacePressStream.Subscribe(_ =>
        {
            _cameraMode = !_cameraMode;

            if (_cameraMode)
            {
                _distance = _inGameDistance;
            }
            else
            {
                _distance = _outGameDistance;
            }
            
        }).AddTo(this);
    }
    
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            _target.position + (_offset * (_distance * 10)),
            Time.deltaTime * _smooth);
    }
}