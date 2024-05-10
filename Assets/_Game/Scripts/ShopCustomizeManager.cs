using Data;
using UnityEngine;
using UniRx;
using Shop;

public class ShopCustomizeManager : MonoBehaviour, IManager<ShopCustomizeManager, EyeCustomizeModel>
{
    public IReactiveProperty<EyeCustomizeModel> CallBack => _callBack;

    #region readonly ReactiveProperties
    private readonly ReactiveProperty<EyeCustomizeModel> _callBack = new();
    #endregion

    [SerializeField] private MeshRenderer _playerEyemeshRenderer;

    [SerializeField] private GameObject _decorContent;
    [SerializeField] private GameObject _decor;

    private Transform _decorParent;

    private DataManager _dataManager;
    
    private ShopViewBase[] _containers;

    private Material _material;
    private SaveSystem _saveSystem;

    private void OnEnable()
    {
        _material = _playerEyemeshRenderer.material;
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
        _saveSystem = MainManager.GetManager<SaveSystem>();
        _dataManager = MainManager.GetManager<DataManager>();
        
        CallBack.Skip(1).Subscribe(data =>
        {
            if (data._eyeDecor >= 0)
            {
                if(_decor){Destroy(_decor);}
                
                var decorData = _dataManager.EyeDecor.DecorParameters[data._eyeDecor];

                _decor = Instantiate(decorData.DecorObject, _decorContent.transform);
            }
            
            _playerEyemeshRenderer.material = EyeShaderGraph.ChangeMaterial(data, _material);

        }).AddTo(this);
        
    }

    private void OnDisable()
    {
        _saveSystem.SaveData();
    }

    private void OnApplicationQuit()
    {
        _saveSystem.SaveData();
    }
}