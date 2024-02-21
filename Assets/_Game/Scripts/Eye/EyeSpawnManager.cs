using System.Collections.Generic;
using System;
using _Game.Scripts.Utility;
using Bot.BotController;
using UniRx;
using UnityEngine;

[Serializable]
public class EyeSpawnList
{
    public BotType BotType;
    public int SpawnCount;
    public BotBehaviourModel BotModel;
}

public class EyeSpawnManager : MonoManager
{
    [SerializeField] private EyeBaseController _botPrrefab;
    [SerializeField] private EyeBaseController _playerTransform;
    [Space] [SerializeField] private UpdateElementController _speedUpdate;
    [SerializeField] private List<EyeSpawnList> _eyeSpawnList;
    public List<EyeBaseController> _spawnedEyes { private set; get; }

    private EyePool _eyePool; 
    
    private IDisposable _spawnBotDisposable;
    private int _index;

    protected override void Awake()
    {
        base.Awake();

        _spawnedEyes = new List<EyeBaseController>();
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
            var position = _playerTransform.transform.position; //get player position

            var randomPosition = new Vector3(position.x, 0, position.z) + // make random position for spawn new element
                                 HelperMath.GetRandomPositionWithClamp(-20, 20, true, 10);

            _lastPosition = randomPosition;

            var state = FreeSpaceCheckManager.CheckVector // Check free space for spawn new element
            (
                randomPosition,
                2, 1 << LayerMask.NameToLayer("Eye")
            );

            bool spawnState = false;

            foreach (var item in _eyeSpawnList)
            {
                if (item.SpawnCount < 1) continue;

                item.SpawnCount--;

                spawnState = true;

                //var spawnElement = Instantiate(_botPrrefab, randomPosition, Quaternion.identity) as EyeBotController; //whithout pooling system

                var spawnElement = _eyePool.GetPoolElement(item.BotType, _botPrrefab as EyeBotController);

                if (spawnElement != null)
                {
                    _spawnedEyes.Add(spawnElement);
                    spawnElement.Activate(item.BotType, item.BotModel); //do this in the last
                }
            }

            if (!spawnState)
            {
                _spawnBotDisposable?.Dispose();
            }
        }).AddTo(this);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_lastPosition, 1);
    }
#endif

}