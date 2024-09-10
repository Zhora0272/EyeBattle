using UnityEngine;
using Zenject;
using UniRx;

public class ScreenToWorldCastView : MonoBehaviour, ISelectedElements
{
    [SerializeField] private Camera _camera;

    //inject
    [Inject] private ScreenToWorldCastController _castController;
    [Inject] private InputController _inputController;

    //ReactiveProperty
    public ReactiveProperty<Collider[]> HitColliders => _hitColliders;
    private readonly ReactiveProperty<Collider[]> _hitColliders = new();

    private void Start()
    {
        _castController.Init(_camera, _inputController, _hitColliders);
    }
}