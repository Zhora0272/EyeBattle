using UnityEngine;

public class SaveSystem : MonoManager
{
    [SerializeField] private EyeCustomizeController _eyeCustomizeController;

    private JsonHelper _dataSave;

    #region ISaveable

    private ISaveable _financeManagerSaveable;
    private ISaveable _eyeCustomizeSaveable;

    #endregion

    #region GameData

    private GameData _financeData;
    private GameData _eyeCustomizeData;

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
    }

    private void Init()
    {
        //init saveable
        _financeManagerSaveable = MainManager.GetManager<FinanceManager>();
        _eyeCustomizeSaveable = _eyeCustomizeController;

        //get data
        _financeData = _financeManagerSaveable.GetData();
        _eyeCustomizeData = _eyeCustomizeSaveable.GetData();
    }

    private void InitData()
    {
        if (!_dataSave.ExistData())
        {
            var data = new GameData
            {
                Money = 100,
                Gem = 15,
                
                EyeConfigModel = new EyeCustomizeModel()
                {
                    _eyeSize = 3.37f,
                    _eyeBibeSize = 2.24f,
                    _eyeColor = new Color(208, 255, 0),
                    _eyeBackColor = new Color(44, 164, 166),
                }
            };

            _dataSave.SaveData(data);
        }

        _gameData = _dataSave.GetData();
    }

    private void SetData()
    {
        _financeData.Money = _gameData.Money;
        _financeData.Gem = _gameData.Gem;
        _eyeCustomizeData.EyeConfigModel = _gameData.EyeConfigModel;

        _financeManagerSaveable.SetData(_financeData);
        _eyeCustomizeSaveable.SetData(_eyeCustomizeData);
    }

    private void OnDestroy()
    {
        SaveData();
    }

    private void OnApplicationFocus(bool state)
    {
        if (Time.time > 5)
        {
            SaveData();
        }
    }

    private void SaveData()
    {
        _dataSave.SaveData(_gameData);
    }
}