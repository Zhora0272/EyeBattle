using UnityEngine;

namespace EyeGunSystem
{
    public class PlayerGunController : MonoBehaviour
    {
        [SerializeField] private BaseBattleParticipant _battleParticipant;
        [SerializeField] private GunController _gunController;
        private InputController _inputController = new();

        private void Awake()
        {
            _gunController = GetComponentInChildren<GunController>();
            _gunController.Init(_battleParticipant);
            
            MainManager.WaitManager<InputController>(manager =>
            {
                _inputController = (InputController)manager;
                
                _inputController.SmartButtonState
                (
                    _gunController._gunType,
                    _gunController._shotType,
                    () => { _gunController.Shoot(); });
            });
        }
    }
}