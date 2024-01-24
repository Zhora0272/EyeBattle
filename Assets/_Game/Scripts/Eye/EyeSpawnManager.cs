using System.Collections.Generic;
using System;
using _Game.Scripts.Utility;
using UniRx;
using UnityEngine;

public class EyeSpawnManager : MonoManager
{
    [SerializeField] private EyeBaseController _botPrrefab;
    [SerializeField] private EyeBaseController _playerTransform;
    [Space] 
    [SerializeField] private UpdateElementController _speedUpdate;
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
            () => { _spawnBotDisposable.Dispose(); });
    }

    private Vector3 _lastPosition;

    private void SpawnEnemies()
    {
        _spawnBotDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            var position = _playerTransform.transform.position;

            var randomPosition = new Vector3(position.x, 0, position.z) +
                                 HelperMath.GetRandomPositionWithClamp(-20, 20, true, 10);

            _lastPosition = randomPosition;

            var state = FreeSpaceCheckManager.CheckVector
            (
                randomPosition,
                2, 1 << LayerMask.NameToLayer("Eye")
            );

            if (!state)
            {
                _spawnCount--;
                if (_spawnCount < 1)
                {
                    _spawnBotDisposable?.Dispose();
                }
                else
                {
                    var item = Instantiate(_botPrrefab, randomPosition, Quaternion.identity);

                    _spawnEyes.Add(item);   
                }
            }

        }).AddTo(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_lastPosition, 1);
    }
}