using UnityEngine;

public abstract class GunController : CachedMonoBehaviour
{
    [SerializeField] protected GunAmmoController ammoController;
    /*[SerializeField] private GunType _gunType;
    [SerializeField] private ShotType _shotType;

    private enum ShotType
    {
        SingleShot,
        Automate,
        SingleTapAutomate,
        AimShot
    }

    private enum GunType
    {
        RocketJetpack,
    }*/

    public abstract void Shoot();

    public void Reload()
    {
        ammoController.TryReloadAmmo();
    }

    private void Awake()
    {
        
    }
}