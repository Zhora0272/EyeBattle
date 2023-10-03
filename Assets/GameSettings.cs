using System;
using TMPro;
using UniRx;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private TextMeshPro _timerText;

    private IDisposable _timerDisposable;

    private int _timerValue;

    private void Start()
    {
        ApplicationSettings();

        MainManager.GetManager<UIManager>().SubscribeToPageActivate(UIPageType.InGame, GameStartDisposableEvents);
        MainManager.GetManager<UIManager>()
            .SubscribeToPageDeactivate(UIPageType.InGame, () => { _timerDisposable.Dispose(); });
    }

    private void GameStartDisposableEvents()
    {
        _timerDisposable = Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            _timerValue++;
            _timerText.text = _timerValue.ToString();
        }).AddTo(this);
    }

    private void ApplicationSettings()
    {
        Application.targetFrameRate = 45;
    }
}