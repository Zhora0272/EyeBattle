using Saveing;
using Shop;
using UnityEngine;

public class EyeCustomizeController : MonoBehaviour, IGameDataSaveable
{
    [SerializeField] private Material _eyeMaterial;
    [SerializeField] private MeshRenderer _eyeMeshRenderer;
    [SerializeField] private GameObject _decorGamobject;

    public Material GetMaterial() => _eyeMaterial;
    public GameObject GetDecor() => _decorGamobject;

    private EyeCustomizeModel _model;

    public void SetData(GameData data)
    {
        _eyeMaterial = EyeShaderGraph.GetMaterial
        (
            data.EyeCustomizeModel
        );
        _model = data.EyeCustomizeModel;
        _eyeMeshRenderer.material = _eyeMaterial;
    }

    public GameData GetData()
    {
        return new GameData()
        {
            EyeCustomizeModel = _model,
        };
    }
}