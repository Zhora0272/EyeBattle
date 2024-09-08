using System.Collections.Generic;
using System;
using _Game.Scripts.Utility;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bot.BotController
{
    public class BotSpawnController
    {
        
    }

    public class BotSpawnManager : MonoManager, IColliderToBotConvertable
    {
        [SerializeField] private Transform _worldTransform;
        [Space] [SerializeField] private MovableBattleParticipantBaseController _botPrrefab;
        [SerializeField] private MineNavMeshAgentController _playerTransform;
        [SerializeField] private List<EyeSpawnList> _eyeSpawnList;

        public List<MineNavMeshAgentController> _spawnedBots { private set; get; }
        private BotPool _botPool;

        private IDisposable _spawnBotDisposable;
        private int _index;

        protected override void Awake()
        {
            base.Awake();
            _botPool = new BotPool();
            _spawnedBots = new List<MineNavMeshAgentController>();
            
            _spawnedBots.Add(_playerTransform);
        }

        private void ReloadSpawnList()
        {
            foreach (var item in _eyeSpawnList)
            {
                item.localSpawnCount = item.SpawnCount;
            }
        }

        public MineNavMeshAgentController SearchBotAnCollider(Collider collider)
        {
            foreach (var item in _spawnedBots)
            {
                if (item.gameObject.GetHashCode() == collider.gameObject.GetHashCode())
                {
                    return item;
                }
            }

            return null;
        }
        
        //need pooling system
        private void Start()
        {
            MainManager.GetManager<UIManager>().SubscribeToPageDeactivate(UIPageType.InGame,
                () => { _spawnBotDisposable?.Dispose(); });
            MainManager.GetManager<UIManager>().SubscribeToPageActivate(UIPageType.InGame, SpawnStart);
        }
        

        private CompositeDisposable _spawnCompositeDisposables;

        private void SpawnStart()
        {
            _spawnCompositeDisposables = new();

            var count = _eyeSpawnList.Count;

            for (int i = 0; i < count; i++)
            {
                var index = i;
                var disposable = Observable.Interval(TimeSpan.FromSeconds(_eyeSpawnList[index].SpawnDelay))
                    .Subscribe(
                        _ => { SpawnBots(index); }).AddTo(this);

                _spawnCompositeDisposables.Add(disposable);
            }
        }

        private void SpawnBots(int index)
        {
            /*ReloadSpawnList();

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
                        }
                    }
                }


                if (!state)
                {
                    var item = _eyeSpawnList[index];

                    bool spawnState = false;

                    if (item.localSpawnCount < 1) return;

                    item.localSpawnCount--;

                    spawnState = true;

                    var spawnElement =
                        _botPool.GetPoolElement(item.BotType, _botPrrefab as MovableBattleParticipantBotController,
                            _worldTransform); // pooling system

                    spawnElement.transform.position = spawnPositions[Random.Range(0, spawnPositions.Count)];

                    spawnElement.SetCustomizeModel(new GameData { EyeCustomizeModel = item.EyeCustomize });

                    if (spawnElement != null)
                    {
                        _spawnedBots.Add(spawnElement);
                        spawnElement.Activate(item.BotType, item.BotModel); //do this in the last
                    }

                    if (!spawnState)
                    {
                        _spawnBotDisposable?.Dispose();
                    }
                }
            }).AddTo(this);*/
        }
    }
}