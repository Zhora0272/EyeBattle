using UnityEngine;

public class EyeCustomizeController : EyeCustomizeModel
{
    [SerializeField] private Material _eyeMaterial;
    [SerializeField] private MeshRenderer _eyeMeshRenderer;

    private void Awake()
    {
        _eyeMeshRenderer.material = EyeShaderGraph.GetMaterial(default);
    }
    
}