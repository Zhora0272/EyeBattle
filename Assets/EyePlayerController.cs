using UnityEngine;

public class EyePlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            _rb.AddTorque(Vector3.right * 50, ForceMode.Acceleration);
        }
    }
}
