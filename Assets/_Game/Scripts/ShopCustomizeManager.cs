using Data;
using UnityEngine;
using UniRx;
using Shop;
using SRF;

public class ShopCustomizeManager : MonoBehaviour, IManager<ShopCustomizeManager, EyeCustomizeModel>
{
    public IReactiveProperty<EyeCustomizeModel> CallBack => _callBack;

    #region readonly ReactiveProperties
    private readonly ReactiveProperty<EyeCustomizeModel> _callBack = new();
    #endregion

    [SerializeField] private MeshRenderer _vetrineEyeMeshRenderer;
    [SerializeField] private MeshRenderer _playerEyemeshRenderer;

    [SerializeField] private GameObject _playerObj;
    [SerializeField] private GameObject _vitrinePlayerObj;
    
    [SerializeField] private GameObject _decorContent;
    [SerializeField] private GameObject _decor;

    private Transform _decorParent;

    private DataManager _dataManager;
    
    private ShopViewBase[] _containers;

    private Material _material;

    private void OnEnable()
    {
        _material = _playerEyemeshRenderer.material;
        _vetrineEyeMeshRenderer.material = _material;
        
        _decorContent.transform.SetParent(_vitrinePlayerObj.transform);
        _decorContent.transform.ResetLocal();
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
        MainManager.GetManager<SaveSystem>().SaveData();
        _decorContent.transform.SetParent(_playerObj.transform);
        _decorContent.transform.ResetLocal();
    }
}