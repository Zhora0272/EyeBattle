using System.Collections.Generic;
using UnityEngine;

public class GunAmmoController : MonoBehaviour
{
    [SerializeField] private List<Transform> _ammoSpawnTransforms;
    [SerializeField] private int _ammoCount;
    [SerializeField] private GunAmmoBase _ammoPrefab;

    private AmmoPool _ammoPool;
    private int _spawnCount;
    
    private void Awake()
    {
        _spawnCount = _ammoSpawnTransforms.Count;
        _ammoPool = new AmmoPool();
    }

    public void TryReloadAmmo()
    {
       
    }

    public bool GetAmmo(out GunAmmoBase ammo)
    {
        if (_ammoCount > 0)
        {
            _ammoCount--;
            ammo = _ammoPool.GetPoolElement(AmmoType.Rocket, _ammoPrefab);
            ammo.PoolActivate();
        }
        else
        {
            ammo = null;
        }
        return ammo;
    }
}
