using UnityEngine;
using UniRx;
using Shop;

public class ShopCustomizeManager : MonoBehaviour, IManager<ShopCustomizeManager, EyeCustomizeModel>
{
    public ReactiveProperty<EyeCustomizeModel> CallBack { get; set; }

    [SerializeField] private MeshRenderer _vetrineEyeMeshRenderer;
    [SerializeField] private MeshRenderer _playerEyemeshRenderer;
    
    private ShopViewBase[] _containers;

    private Material _material;

    private void Awake()
    {
        CallBack = new ReactiveProperty<EyeCustomizeModel>();
    }

    private void OnEnable()
    {
        _material = _playerEyemeshRenderer.material;
        _vetrineEyeMeshRenderer.material = _material;
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
            _playerEyemeshRenderer.material = EyeShaderGraph.ChangeMaterial(value, _material);

        }).AddTo(this);
    }
}