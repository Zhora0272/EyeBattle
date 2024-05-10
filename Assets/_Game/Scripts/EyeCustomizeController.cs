using System;
using Data;
using Saveing;
using UniRx;
using UnityEngine;

public class EyeCustomizeController : MonoBehaviour, IGameDataSaveable
{
    [SerializeField] private Material _eyeMaterial;
    [SerializeField] private MeshRenderer _eyeMeshRenderer;
    [SerializeField] private GameObject _decorGamobject;

    [SerializeField] private DataManager _dataManager;
    public Material GetMaterial() => _eyeMaterial;
    public ReactiveProperty<GameObject> ReactiveDecorGameObject => new();

    [SerializeField] private EyeCustomizeModel _model;

    private void Start()
    {
        _dataManager ??= MainManager.GetManager<DataManager>();
    }

    public void SetData(GameData data)
    {
        _eyeMaterial = EyeShaderGraph.GetMaterial(data.EyeCustomizeModel);
        ReactiveDecorGameObject.Value = _dataManager.EyeDecor.DecorParameters[data.EyeCustomizeModel._eyeDecor].DecorObject;
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