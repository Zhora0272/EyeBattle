using UniRx;
using UnityEngine;

public class PlayerGunController : MonoBehaviour
{
    private GunController _gunController;
    private ReactiveProperty<InputController> _inputController = new();

    private void Awake()
    {
        _gunController = GetComponentInChildren<GunController>();

        MainManager.WaitManager<InputController>(manager =>
        {
            _inputController.Value = (InputController)manager;
        });
    }

    private void Start()
    {
        _inputController.Subscribe(value =>
        {
            if(!value) return;
            
            _inputController.Value.SmartButtonState
            (
                _gunController._gunType,
                _gunController._shotType,
                () =>
                {
                    _gunController.Shoot();
                });
        }).AddTo(this);
    }
}