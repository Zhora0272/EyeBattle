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

public abstract class GunController : CachedMonoBehaviour
{
    [SerializeField] protected GunAmmoController ammoController;
    [SerializeField] internal GunType _gunType;
    [SerializeField] internal ShotType _shotType;

    public abstract void Shoot();

    public void Reload()
    {
        ammoController.TryReloadAmmo();
    }

    private void Awake()
    {
    }
}