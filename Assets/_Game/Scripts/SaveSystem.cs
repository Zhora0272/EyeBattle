using System;
using UniRx;
using UnityEngine;

public class SaveSystem : MonoManager
{
    [SerializeField] private EyeCustomizeController _eyeCustomizeController;

    private JsonHelper _dataSave;

    #region ISaveable

    private ISaveable _financeManagerSaveable;
    private ISaveable _eyeCustomizeSaveable;

    #endregion

    private GameData _gameData;

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

        Observable.Interval(TimeSpan.FromSeconds(10)).Subscribe(_ =>
        {
            SaveData();
        }).AddTo(this);
    }

    private void Init()
    {
        //init saveable
        _financeManagerSaveable = MainManager.GetManager<FinanceManager>();
        _eyeCustomizeSaveable = _eyeCustomizeController;
    }

    private void InitData()
    {
        if (!_dataSave.ExistData())
        {
            //default configurations
            var data = new GameData
            {
                Money = 100,
                Gem = 15,
                
                EyeConfigModel = new EyeCustomizeModel()
                {
                    _eyeSize = 3.37f,
                    _eyeBibeSize = 2.24f,
                    _eyeColor = 1,
                    _eyeBackColor = 2,
                }
            };

            _dataSave.SaveData(data);
        }

        _gameData = _dataSave.GetData();
    }
    private void SetData()
    {
        var financeData = _financeManagerSaveable.GetData();
        var playerEyeData = _eyeCustomizeSaveable.GetData();
        
        financeData.Money = _gameData.Money;
        financeData.Gem = _gameData.Gem;
        
        
        playerEyeData.EyeConfigModel = _gameData.EyeConfigModel;

        _financeManagerSaveable.SetData(financeData);
        _eyeCustomizeSaveable.SetData(playerEyeData);
    }

    private void SaveData()
    {
        var financeData = _financeManagerSaveable.GetData();
        var playerEyeData = _eyeCustomizeSaveable.GetData();
        
        _gameData.Gem = financeData.Gem;
        _gameData.Money = financeData.Money;
        _gameData.EyeConfigModel = _eyeCustomizeSaveable.GetData().EyeConfigModel;
        
        _dataSave.SaveData(_gameData);
    }
}