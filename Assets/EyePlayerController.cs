using UnityEngine;

public abstract class EyeBaseController : MonoBehaviour 
{
    [SerializeField] protected BrokenEyePartsController _brokenEyePartsController;
    [SerializeField] protected Rigidbody Rb;
    [SerializeField] protected int Hp;
    [Space]
    [SerializeField] private Material _material;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private SphereCollider _sphereCollider;

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

public class EyePlayerController : EyeBaseController
{
    [SerializeField] private InputController _inputController;
   
    private Vector3 _lastPosition;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();

        _inputController.RegisterJoysticData(data =>
        {
            _moveDirection = data;

        });
    }

    private void Update()
    {
        DirectionCalculation();
    }

    private void FixedUpdate()
    {
        if (_moveDirection != Vector2.zero)
        {
            Rb.AddTorque(
                new Vector3(_moveDirection.y, 0, -_moveDirection.x)
                * 50,
                ForceMode.Acceleration);
        }
    }

    private void DirectionCalculation()
    {
        var lastDirection = _lastPosition - transform.position;

        _lastPosition = transform.position;
    }
}
