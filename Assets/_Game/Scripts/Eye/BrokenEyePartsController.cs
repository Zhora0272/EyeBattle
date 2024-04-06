using UnityEngine;

public class BrokenEyePartsController : MonoBehaviour
{
    [SerializeField] private Transform[] _transforms;
    [SerializeField] private Rigidbody[] _partsRb;
    [SerializeField] private Renderer[] _partsMaterial;

    [SerializeField] private Vector3[] _partPositions;
    [SerializeField] private Quaternion[] _partRotations;

    private void OnValidate()
    {
        
    }

    public void Activate(Material mat, Vector3 activatePosition)
    {
        transform.SetParent(null);
        gameObject.SetActive(true);
        
        for (int i = 0; i < _partsRb.Length; i++)
        {
            _partsRb[i].isKinematic = false;
            _partsMaterial[i].material = mat;
        }
    }
}
