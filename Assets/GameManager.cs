using System;
using Bot.BotController;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MoveableBattleParticipantController controller;
    [SerializeField] private BotSpawnManager _spawnManager;
    [SerializeField] private UIManager _uiManager;

    private void Start()
    {
        _uiManager = MainManager.GetManager<UIManager>();
        _spawnManager = MainManager.GetManager<BotSpawnManager>();
    }
}