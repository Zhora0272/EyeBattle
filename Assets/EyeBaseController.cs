using UnityEngine;

public abstract class EyeBaseController : MonoBehaviour 
{
    [SerializeField] protected BrokenEyePartsController _brokenEyePartsController;
    [SerializeField] protected Rigidbody Rb;
    [SerializeField] protected int Hp;
    [Space]
    [SerializeField] protected Material _material;
    [SerializeField] protected MeshRenderer _meshRenderer;
    [SerializeField] protected SphereCollider _sphereCollider;
    [Space]
    [SerializeField] protected float Speed;

    protected Vector2 _moveDirection;

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Eye"))
        {
            if(collision.gameObject.TryGetComponent<EyeBaseController>(out var result))
            {
                result.Attack(Rb.mass * Rb.velocity.magnitude);
            }
        }
    }

    protected virtual void Update()
    {
        Rb.velocity = Vector3.Lerp(Rb.velocity, Vector3.zero, Time.deltaTime);
        Rb.angularVelocity = Vector3.Lerp(Rb.angularVelocity, Vector3.zero, Time.deltaTime);
    }

    protected virtual void Attack(float force)
    {
        if(Hp < force)
        {
            _brokenEyePartsController.Activate(_material);
            _sphereCollider.enabled = false;
            _meshRenderer.enabled = false;
            Rb.isKinematic = true;
        }
    }

}
