using UnityEngine;

public class HeadGunAmmoController : GunAmmoBase
{
    private Rigidbody _rb;

    private void Awake()
    {
        _rb ??= GetComponent<Rigidbody>();
    }

    internal override void Attack(ITransform target)
    {
        transform.SetParent(null);
        _rb.isKinematic = false;
        _rb.AddRelativeForce(Vector3.forward, ForceMode.Impulse);
        _rb.AddRelativeTorque(Vector3.forward * 150);
    }

    protected override void Explosion()
    {
        print("explosion is not implemented");
    }
}