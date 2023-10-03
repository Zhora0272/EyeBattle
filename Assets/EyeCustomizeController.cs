using UnityEngine;

public class EyeCustomizeController : MonoBehaviour
{
    [SerializeField] private Material _eyeMaterial;
    [SerializeField] private MeshRenderer _eyeMeshRenderer;
    [SerializeField] private GameObject _decorGamobject;

    private void Awake()
    {
        var model = new EyeCustomizeModel();
        
        model._eyeColor = Color.green;
        model._eyeBackColor = Color.white;
        model._eyeSize = 1;
        
        _eyeMaterial = EyeShaderGraph.GetMaterial(model);
        _eyeMeshRenderer.material = _eyeMaterial;
    }
    
    public Material GetMaterial() => _eyeMaterial;
    public GameObject GetDecor() => _decorGamobject;
}

