using UniRx;
using UnityEngine;

public enum GunType
{
    RocketJetpack,
}

public enum ShotType
{
    SingleShot,
    Automate,
    SingleTapAutomate,
    AimShot
}

namespace EyeGunSystem
{
    public abstract class GunController : CachedMonoBehaviour
    {
        [SerializeField] protected GunAmmoController ammoController;
        [SerializeField] internal GunType _gunType;
        [SerializeField] internal ShotType _shotType;
        
        protected  ReactiveProperty<BattleParticipantsManager> battleManager = new();
        protected ReactiveProperty<BaseBattleParticipant> battleParticipant = new();
        
        internal void Init(BaseBattleParticipant participant)
        {
            print(participant + "Init");
            
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
                print(manager);
                
                battleParticipant.Subscribe(value =>
                {
                    print(value);
                    if(!value) return;
                
                    print(value);
                    battleManager.Value.GetClosest(value.EyeParameters, out var result);
                
                    print(value.EyeParameters);
                    print(result);
                
                }).AddTo(this);
            });
        }
    }
}