using UnityEngine;

public class BrokenEyePartsController : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Transform[] _transforms;
    [SerializeField] private Rigidbody[] _partsRb;
    [SerializeField] private Renderer[] _partsMaterial;

    [SerializeField] private Mesh[] _partsMeshs;

    [SerializeField] private Vector3[] _partPositions;

    [ContextMenu("set positions")]
    private void SetPartsPositions()
    {
        
    }
    
    /*[ContextMenu("get positions")]
    private void GetPartsPositions()
    {
        _partPositions = new Vector3[_transforms.Length];

        for (int i = 0; i < _transforms.Length; i++)
        {
            _partPositions[i] = _transforms[i].position;
        }
    }*/

    [ContextMenu("Set Meshs")]
    private void Init()
    {
        for (int i = 0; i < _transforms.Length; i++)
        {
            _transforms[i].GetComponent<MeshFilter>().mesh = _partsMeshs[i];
            _transforms[i].GetComponent<MeshCollider>().sharedMesh = _partsMeshs[i];
            _transforms[i].position = _partPositions[i];
        }
    }

    public void Activate()
    {
        transform.SetParent(null);
        gameObject.SetActive(true);
        
        for (int i = 0; i < _partsRb.Length; i++)
        {
            _partsRb[i].isKinematic = false;
            _partsMaterial[i].material = _meshRenderer.material;
        }
    }
}
