using UnityEngine;
using System;
using System.Linq;
using TMPro;
using UniRx;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public class SelectablePlane : MonoBehaviour, ISelectableManager
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _mergeCountText;
    [SerializeField] private TextMeshProUGUI _bestScoreText;
    
    private SelectableGrid[] _selectableGrid;
    [field:SerializeField] public Data2048 Data { get; private set; }
    public Action MergeCallback { get; private set; }

    private IDisposable _spawnElementDisposable;
    
    private int _mergeCount;
    private int _bestScore;
    private int _currentScore;

    private enum PlayerPrefsEnum
    {
        WeaponMergeSaveIndex,
        InitialStart,
        BestScore,
        Score
    }

    private void Awake()
    {
        _selectableGrid = GetComponentsInChildren<SelectableGrid>();
    }

    private bool _skipSpawn;
    private void Start()
    {
        MainManager.GetManager<UIManager>().SubscribeToPageActivate(UIPageType.Shop,
            () => { _spawnElementDisposable?.Dispose(); });

        MainManager.GetManager<UIManager>().SubscribeToPageDeactivate(UIPageType.Shop,
            () => { StartSpawnRandomElements(); });

        _scoreText.text = "0";

        _bestScore = PlayerPrefs.GetInt(PlayerPrefsEnum.Score.ToString());

        _bestScoreText.text = _bestScore.ToString();

        StartGameProcess();
    }

    private void StartGameProcess()
    {
        var initialState = PlayerPrefs.GetInt(PlayerPrefsEnum.InitialStart.ToString());

        for (int i = 0; i < _selectableGrid.Length; i++)
        {
            _selectableGrid[i].SetManager(this, Data);
        }

        PlayerPrefs.SetInt(PlayerPrefsEnum.InitialStart.ToString(), 1);

        if (initialState == 1)
        {
            for (int i = 0; i < _selectableGrid.Length; i++)
            {
                var index = PlayerPrefs.GetInt(PlayerPrefsEnum.WeaponMergeSaveIndex.ToString() + i, -1);
                if (index != -1)
                {
                    _selectableGrid[i].SetObject(index);

                    if (_currentScore == -1) continue;
                    _currentScore += index;
                }
            }

            _currentScore *= 10;

            _scoreText.text = _currentScore.ToString();
        }
        else
        {
            _selectableGrid[0].SetObject();
            _selectableGrid[Random.Range(1, _selectableGrid.Length - 1)].SetObject();
        }

        MergeCallback = () =>
        {
            _currentScore = 0;

            _mergeCount++;

            if (Time.frameCount % 2 == 0) return;

            Observable.Timer(
                TimeSpan.FromSeconds(
                    Random.Range(0.2f, 0.8f))
                ).Subscribe(_ =>
                {
                    RandomSpawn();

                }).AddTo(this);


           
            foreach (var item in _selectableGrid)
            {
                if (_currentScore == -1) continue;

                _currentScore += item._stayObjectIndex;
            }

            _currentScore *= 10;

            if(_currentScore > _bestScore)
            {
                _bestScore = _currentScore;
            }

            _bestScoreText.text = _bestScore.ToString();

            _mergeCountText.text = _mergeCount.ToString();

            _scoreText.text = _currentScore.ToString();

        };

        StartSpawnRandomElements();
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt(PlayerPrefsEnum.Score.ToString(), _currentScore);
    }

    private void StartSpawnRandomElements()
    {
        _spawnElementDisposable = Observable.Interval(
            TimeSpan.FromSeconds(Random.Range(5f, 10f))
            ).Subscribe(_ =>
        {
            if (_skipSpawn)
            {
                _skipSpawn = false;
                return;
            }

            RandomSpawn();

        }).AddTo(this);
    }

    private float _lastSpawnTime;
    
    private void RandomSpawn()
    {
        if (Time.time - _lastSpawnTime < 1) return;
        _lastSpawnTime = Time.time;

        var freeGrides = from item in _selectableGrid where (!item.CheckObject()) select (item);

        var count = freeGrides.Count();
        var randomIndex = Random.Range(0, count - 1);

        if (freeGrides.Count() > 0)
        {
            freeGrides.ToArray()[randomIndex].SetObject();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _selectableGrid.Length; i++)
        {
            PlayerPrefs.SetInt(PlayerPrefsEnum.WeaponMergeSaveIndex.ToString() + i,
                _selectableGrid[i].GetUpgradeIndex());
        }
    }
}

public interface ISelectableManager
{
    public Data2048 Data { get; }
    public Action MergeCallback { get; }  
}