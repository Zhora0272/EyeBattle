using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _distance;

    private void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            _target.position + ((_offset * 10) * _distance),
            Time.deltaTime);
    }
}
