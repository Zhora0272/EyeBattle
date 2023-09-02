using UnityEngine;

public class BrokenEyePartsController : MonoBehaviour
{
    [SerializeField] private Transform[] _transforms;
    [SerializeField] private Rigidbody[] _partsRb;
    [SerializeField] private Renderer[] _partsMaterial;

    public void Activate(Material mat, Vector3 activatePosition)
    {
        gameObject.SetActive(true);

        for (int i = 0; i < _partsRb.Length; i++)
        {
            _partsRb[i].isKinematic = false;
            _partsMaterial[i].material = mat;

            //_partsRb[i].AddRelativeForce(Vector3.up * 150, ForceMode.Impulse);
        }
    }
}
