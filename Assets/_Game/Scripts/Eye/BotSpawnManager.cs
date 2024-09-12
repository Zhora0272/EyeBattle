using System.Collections.Generic;
using System;
using Bot.BotController;
using UniRx;
using UnityEngine;
using Zenject;

public class BotSpawnManager : MonoBehaviour, IColliderToBotConvertable
{
    
    [Inject] private BattleEnemyParticipantPooling _enemyParticipantPooling;
    [Inject] private BattleTeammateParticipantPooling _teammateParticipantPooling;

    [SerializeField] private MoveableBattleParticipantBaseController _playerTransform;
    [SerializeField] private List<EyeSpawnList> _eyeSpawnList;

    public List<MoveableBattleParticipantBaseController> _spawnedBots { private set; get; }

    private IDisposable _spawnBotDisposable;
    private int _index;


    protected void Awake()
    {
        _spawnedBots = new List<MoveableBattleParticipantBaseController> { _playerTransform };
    }

    private void Start()
    {
        Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            var bot = _enemyParticipantPooling.Spawn();
            bot.transform.position = Vector3.zero;

        }).AddTo(this);
    }

    public MoveableBattleParticipantBaseController SearchBotWithCollider(Collider collider)
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
}