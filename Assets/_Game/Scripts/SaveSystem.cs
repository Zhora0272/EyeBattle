using UnityEngine;
using System;
using UniRx;

public class SaveSystem : MonoManager
{
    [SerializeField] private SettingsView _settingsView;
    private FinanceManager _financeManager;

    private GameData _data;

    private int _openLevelCount;
    private JsonHelper _dataSave;


    protected override void Awake()
    {
        base.Awake();
        _dataSave = new JsonHelper();
        
        Observable.Interval(TimeSpan.FromSeconds(30)).Subscribe(_ =>
        {
#if !UNITY_EDITOR
         var tutorialState = PlayerPrefs.GetInt(PlayerPrefsEnum.TutorialState.ToString());
         if (tutorialState == 1)
         {
             SaveData();
         }
#else
            SaveData();
#endif
        });
    }

    public void TrySave()
    {
        SaveData();
    }
    
    private void Start()
    {
        _financeManager = MainManager.GetManager<FinanceManager>();


        if (!_dataSave.ExistData())
        {
            var data = new GameData();

            _dataSave.SaveData(data);
        }

        _data = _dataSave.GetData();
        
    }


    private void OnDestroy()
    {
#if !UNITY_EDITOR
         var tutorialState = PlayerPrefs.GetInt(PlayerPrefsEnum.TutorialState.ToString());
         if (tutorialState == 1)
         {
             SaveData();
         }
#else
        SaveData();
#endif
    }

    private void OnApplicationFocus(bool state)
    {
#if !UNITY_EDITOR
        var tutorialState = PlayerPrefs.GetInt(PlayerPrefsEnum.TutorialState.ToString());
        if (Time.time > 5 && tutorialState == 1)
        {
            SaveData();
        }
#else
        if (Time.time > 5)
        {
            SaveData();
        }
#endif
    }

    private void SaveData()
    {
        _dataSave.SaveData(_data);
    }
}