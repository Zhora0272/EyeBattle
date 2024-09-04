using System;
using Bot.BotController;
using UniRx;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private NpcPlayerController _playerController;
    [SerializeField] private EyeSpawnManager _spawnManager;
    [SerializeField] private UIManager _uiManager;

    private void Start()
    {
        _uiManager = MainManager.GetManager<UIManager>();
        _spawnManager = MainManager.GetManager<EyeSpawnManager>();
        
        _uiManager.SubscribeToPageActivate(UIPageType.InGame, () =>
        {
            _playerController.EyeActivate();
        });

        _uiManager.SubscribeToPageActivate(UIPageType.TapToPlay, () =>
        {
            _spawnManager.CrushAllEyeBots();
        });
        
        _playerController.IsDeath.Subscribe(state =>
        {
            if (state)
            {
                _spawnManager.CrushAllEyeBots();
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ =>
                {
                    _uiManager.Activate(UIPageType.TapToPlay);
                    
                }).AddTo(this);
            }
        }).AddTo(this);
    }
}