using Shop.Container;
using System;
using UniRx;
using UnityEngine;

public class SaveSystem : MonoManager
{
    [SerializeField] private EyeCustomizeController _eyeCustomizeController;
    [SerializeField] private ShopContainerManager _shopContainerManager;

    private JsonHelper _dataSave;

    #region ISaveable

    private ISaveable _financeManagerSaveable;
    private ISaveable _eyeCustomizeSaveable;
    private ISaveable _shopContainerSaveable;

    #endregion

    //
    private GameData _gameData;
    
    //
    private DataManager _dataManager;

    protected override void Awake()
    {
        base.Awake();

        _dataSave = new JsonHelper();
    }

    private void Start()
    {
        Init();
        InitData();
        SetData();

        Observable.Interval(TimeSpan.FromSeconds(4)).Subscribe(_ =>
        {
            SaveData();
            
        }).AddTo(this);
    }

    private void Init()
    {
        _financeManagerSaveable = MainManager.GetManager<FinanceManager>();
        _dataManager = MainManager.GetManager<DataManager>();
        
        _eyeCustomizeSaveable = _eyeCustomizeController;
        _shopContainerSaveable = _shopContainerManager;
    }

    private void InitData()
    {
        #region Set Deafult Variables For First
        //set default variables
        if (!_dataSave.ExistData())
        {
            var data = new GameData
            {
                Money = 100,
                Gem = 15,
                ContainerConfigIndexes = new[] {0, 0, 0, 0},
                EyeItemParameters = _dataManager.GetAllDataLists(),
                EyeConfigModel = new EyeCustomizeModel
                {
                    _eyeSize = 3.37f,
                    _eyeBibeSize = 2.24f,
                    _eyeColor = 1,
                    _eyeBackColor = 2
                }
            };
            _dataSave.SaveData(data);
        }
        #endregion

        _gameData = _dataSave.GetData();
    }

    private void SetData()
    {
        var financeData = _financeManagerSaveable.GetData();
        var playerEyeData = _eyeCustomizeSaveable.GetData();
        var containerManager = _shopContainerSaveable.GetData();

        financeData.Money = _gameData.Money;
        financeData.Gem = _gameData.Gem;
        playerEyeData.EyeConfigModel = _gameData.EyeConfigModel;
        containerManager.ContainerConfigIndexes = _gameData.ContainerConfigIndexes;

        _financeManagerSaveable.SetData(financeData);
        _eyeCustomizeSaveable.SetData(playerEyeData);
        _shopContainerSaveable.SetData(containerManager);
        
    } 

    public void SaveData()
    {
        var financeData = _financeManagerSaveable.GetData();
        var playerEyeData = _eyeCustomizeSaveable.GetData();
        var containerManager = _shopContainerSaveable.GetData();

        _gameData.Gem = financeData.Gem;
        _gameData.Money = financeData.Money;
        _gameData.EyeConfigModel = _eyeCustomizeSaveable.GetData().EyeConfigModel;
        _gameData.ContainerConfigIndexes = containerManager.ContainerConfigIndexes;

        _gameData.EyeItemParameters = new[]
        {
            new EyeItemCollection(){ BaseEyeItems = _dataManager.EyeColor.Colors},
            new EyeItemCollection(){ BaseEyeItems = _dataManager.EyeBackColor.Colors},
        };

        _dataSave.SaveData(_gameData);
    }
}