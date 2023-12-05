using System.Collections.Generic;
using System;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class EyeSpawnManager : MonoManager
{
    [SerializeField] private EyeBaseController _botPrrefab;
    [SerializeField] private EyeBaseController _playerTransform;
    [SerializeField] private int _spawnCount;
    public List<EyeBaseController> _spawnEyes { private set; get; }

    private IDisposable _spawnBotDisposable;
    private int _index;

    protected override void Awake()
    {
        base.Awake();

        _spawnEyes = new List<EyeBaseController>();
    }

    //need pooling system

    private void Start()
    {
        MainManager.GetManager<UIManager>().SubscribeToPageActivate(UIPageType.InGame, SpawnEnemies);

        MainManager.GetManager<UIManager>().SubscribeToPageDeactivate(UIPageType.InGame,
            () =>
            {
                _spawnBotDisposable.Dispose();
            });
    }

    private void SpawnEnemies()
    {
        _spawnBotDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            var position = _playerTransform.transform.position;

            var randomPosition = new Vector3(position.x, 0, position.z) +
                                 new Vector3(Random.Range(-10, -5), 0, Random.Range(5, 10));

            if (FreeSpaceCheckManager.CheckVector(randomPosition))
            {
                var item = Instantiate(_botPrrefab, randomPosition, Quaternion.identity);

                _spawnEyes.Add(item);

                _spawnCount++;   
            }
        }).AddTo(this);
    }
}