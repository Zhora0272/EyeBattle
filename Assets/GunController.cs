using UniRx;
using UnityEngine;

public enum GunType
{
    RocketJetpack,
    HeadGun,
}

public enum ShotType
{
    SingleShot,
    Automate,
    SingleTapAutomate,
    AimShot, 
    ShootAllAmmo
}

namespace EyeGunSystem
{
    public abstract class GunController : CachedMonoBehaviour
    {
        [SerializeField] protected GunAmmoController ammoController;
        [SerializeField] internal GunType _gunType;
        [SerializeField] internal ShotType _shotType;

        protected ReactiveProperty<BattleParticipantsManager> battleManager = new();
        protected ReactiveProperty<BaseBattleParticipant> battleParticipant = new();

        internal void Init(BaseBattleParticipant participant)
        {
            battleParticipant.Value = participant;
        }

        public abstract void Shoot();

        public void Reload()
        {
            ammoController.TryReloadAmmo();
        }

        protected virtual void Awake()
        {
            MainManager.WaitManager<BattleParticipantsManager>(manager =>
            {
                battleManager.Value = (BattleParticipantsManager)manager;
            });
        }
    }
}