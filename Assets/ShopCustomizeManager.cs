using UnityEngine;
using UniRx;
using Shop;

public class ShopCustomizeManager : MonoBehaviour, IManager<ShopCustomizeManager, EyeCustomizeModel>
{
    [SerializeField] private MeshRenderer _vetrineEyeMeshRenderer;
    private ShopViewBase[] _containers;

    private void Awake()
    {
        CallBack = new ReactiveProperty<EyeCustomizeModel>();
    }

    private void Start()
    {
        _containers = GetComponentsInChildren<ShopViewBase>();
        
        foreach (ShopViewBase container in _containers)
        {
            container.SetManager(this);
        }

        CallBack.Skip(1).Subscribe(value =>
        {
            _vetrineEyeMeshRenderer.material = EyeShaderGraph.GetMaterial(value);
            EyeShaderGraph.ChangeMaterial(value,_vetrineEyeMeshRenderer.material);
        }).AddTo(this);
    }

    public ReactiveProperty<EyeCustomizeModel> CallBack { get; set; }
}