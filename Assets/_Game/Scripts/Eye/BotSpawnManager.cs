using System.Collections.Generic;
using System;
using _Game.Scripts.Utility;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bot.BotController
{
    public class BotSpawnManager : MonoManager
    {
        [SerializeField] private Transform _worldTransform;
        [Space] [SerializeField] private MovableBattleParticipantBaseController _botPrrefab;
        [SerializeField] private MovableBattleParticipantBaseController _playerTransform;
        [Space] [SerializeField] private UpdateElementController _speedUpdate;
        [SerializeField] private List<EyeSpawnList> _eyeSpawnList;


        public List<MovableBattleParticipantBaseController> _spawnedEyes { private set; get; }
        private EyePool _eyePool;

        private IDisposable _spawnBotDisposable;
        private int _index;

        protected override void Awake()
        {
            base.Awake();
            _eyePool = new EyePool();
            _spawnedEyes = new List<MovableBattleParticipantBaseController>();
        }

        private void ReloadSpawnList()
        {
            foreach (var item in _eyeSpawnList)
            {
                item.localSpawnCount = item.SpawnCount;
            }
        }

        //need pooling system
        private void Start()
        {
            MainManager.GetManager<UIManager>().SubscribeToPageDeactivate(UIPageType.InGame,
                () => { _spawnBotDisposable?.Dispose(); });
            MainManager.GetManager<UIManager>().SubscribeToPageActivate(UIPageType.InGame, SpawnStart);
        }

        internal void CrushAllEyeBots()
        {
            _spawnCompositeDisposables?.Dispose();
            
            if (_spawnedEyes.Count <= 0) return;

            this.WaitAndDoCycle(_spawnedEyes.Count - 1, .01f, i => { _spawnedEyes[i].DeadEvent(); });

            _spawnBotDisposable?.Dispose();
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
                        _ => { SpawnEnemies(index); }).AddTo(this);

                _spawnCompositeDisposables.Add(disposable);
            }
        }

        private void SpawnEnemies(int index)
        {
            ReloadSpawnList();

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
                        _eyePool.GetPoolElement(item.BotType, _botPrrefab as MovableBattleParticipantBotController,
                            _worldTransform); // pooling system

                    spawnElement.transform.position = spawnPositions[Random.Range(0, spawnPositions.Count)];

                    spawnElement.SetCustomizeModel(new GameData { EyeCustomizeModel = item.EyeCustomize });

                    if (spawnElement != null)
                    {
                        _spawnedEyes.Add(spawnElement);
                        spawnElement.Activate(item.BotType, item.BotModel); //do this in the last
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