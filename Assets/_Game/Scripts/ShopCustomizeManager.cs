using UnityEngine;
using UniRx;
using Shop;

public class ShopCustomizeManager : MonoBehaviour, IManager<ShopCustomizeManager, EyeCustomizeModel>
{
    public IReactiveProperty<EyeCustomizeModel> CallBack => _callBack;

    #region readonly ReactiveProperties
    private readonly ReactiveProperty<EyeCustomizeModel> _callBack = new();
    #endregion

    [SerializeField] private MeshRenderer _vetrineEyeMeshRenderer;
    [SerializeField] private MeshRenderer _playerEyemeshRenderer;
    
    private ShopViewBase[] _containers;

    private Material _material;

    private void OnEnable()
    {
        _material = _playerEyemeshRenderer.material;
        _vetrineEyeMeshRenderer.material = _material;
    }

    private void Awake()
    {
        _containers = GetComponentsInChildren<ShopViewBase>();
        
        foreach (ShopViewBase container in _containers)
        {
            container.InitShopView(this);
        }
    }

    private void Start()
    {
        CallBack.Skip(1).Subscribe(data =>
        {
            _playerEyemeshRenderer.material = EyeShaderGraph.ChangeMaterial(data, _material);

        }).AddTo(this);
        
    }

    private void OnDisable()
    {
        MainManager.GetManager<SaveSystem>().SaveData();
    }
}