using UnityEngine;
using Zenject;
using UniRx;

public class ScreenToWorldCastView : MonoBehaviour, ISelectedElements
{
    [Inject] private ScreenToWorldCastController _castController;

    [SerializeField] private InputController _inputController;

    [Space] [SerializeField] private Camera _camera;
    public ReactiveProperty<Collider[]> HitColliders => _hitColliders;
    private readonly ReactiveProperty<Collider[]> _hitColliders = new();

    private void Start()
    {
        _castController.Init(_camera, _inputController);
        
        _hitColliders.Subscribe(colliders =>
            { _castController.SetHitColliders(colliders); }).AddTo(this);
    }
}