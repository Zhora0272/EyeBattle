using System.Collections.Generic;
using System;
using System.Collections;
using _Game.Scripts.Utility;
using Shop;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bot.BotController
{
    public class EyeSpawnManager : MonoManager
    {
        [SerializeField] private Transform _worldTransform;
        [Space] [SerializeField] private EyeBaseController _botPrrefab;
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
            _eyePool = new EyePool();
            _spawnedEyes = new List<EyeBaseController>();
        }

        //need pooling system
        private void Start()
        {
            MainManager.GetManager<UIManager>().SubscribeToPageActivate(UIPageType.InGame, SpawnEnemies);
            MainManager.GetManager<UIManager>().SubscribeToPageDeactivate(UIPageType.InGame,
                () => { _spawnBotDisposable?.Dispose(); });
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_randomPosition + Vector3.up, 2);
        }

        private Vector3 _randomPosition;

        private void SpawnEnemies()
        {
            List<Vector3> spawnPositions = new();
            
            _spawnBotDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
            {
                spawnPositions.Clear();
                
                bool state = true;

                int distance = 55;

                var position = _playerTransform.transform.position; //get player position

                for (int j = -distance; j < distance; j++)
                {
                    for (int i = -distance; i < distance * 1.20; i++)
                    {
                        var randomPosition = new Vector3(position.x, 0, position.z) + new Vector3(i, 0, j);
                        var magnitude = (position - randomPosition).magnitude;
                        if (magnitude < 40 || magnitude > 50) continue;

                        state = FreeSpaceCheckManager.CheckVector // Check free space for spawn new element
                        (
                            randomPosition,
                            3, ~ (1 << 3)
                        );

                        if (!state)
                        {
                            spawnPositions.Add(randomPosition);
                            _randomPosition = randomPosition;
                        }
                    }
                }


                if (!state)
                {
                    bool spawnState = false;

                    foreach (var item in _eyeSpawnList)
                    {
                        if (item.SpawnCount < 1) continue;

                        item.SpawnCount--;

                        spawnState = true;

                        var spawnElement =
                            _eyePool.GetPoolElement(item.BotType, _botPrrefab as EyeBotController,
                                _worldTransform); // pooling systeam

                        spawnElement.transform.position = spawnPositions[Random.Range(0,spawnPositions.Count)];
                        
                        spawnElement.SetCustomizeModel(new GameData { EyeCustomizeModel = item.EyeCustomize });

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
                }
            }).AddTo(this);
        }
    }
}