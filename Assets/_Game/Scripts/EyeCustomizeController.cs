using Saveing;
using Shop;
using UnityEngine;

public class EyeCustomizeController : MonoBehaviour, IGameDataSaveable
{
    [SerializeField] private Material _eyeMaterial;
    [SerializeField] private MeshRenderer _eyeMeshRenderer;
    [SerializeField] private GameObject _decorGamobject;
    [SerializeField] private Texture _texture;

    public Material GetMaterial() => _eyeMaterial;
    public GameObject GetDecor() => _decorGamobject;

    public void SetData(GameData data)
    {
        _eyeMaterial = EyeShaderGraph.GetMaterial
        (
            data.EyeConfigModel
        );
        _eyeMeshRenderer.material = _eyeMaterial;
    }

    public GameData GetData()
    {
        var model = EyeShaderGraph.ConvertMaterialToModel(_eyeMeshRenderer.material);
        return new GameData()
        {
            EyeConfigModel = model,
        };
    }
}